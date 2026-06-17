class GameBoard extends HTMLElement {
    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
        this.boardsInitialized = false;
        this.userName = '';
        this.currentTurnPlayerId = null;
        this.gameOver = false;
        this.hitsMade = 0;
        this.hitsReceived = 0;
        this.connectionReady = false;
        this.render();
        this.setupSignalR();
    }

    render() {
        this.shadowRoot.innerHTML = `
            <link rel="stylesheet" href="/css/zeeslag-game.css">
            <div id="container">
                <div id="chat">
                    <div id="messages"></div>
                    <div id="chatInput">
                        <input type="text" id="messageInput" placeholder="Type a message...">
                        <button id="sendButton">Send</button>
                    </div>
                </div>
                <div id="boardLabels">
                    <div class="board-label-group">
                        <span class="board-title">Jouw Bord</span>
                        <span id="hitsReceived" class="hit-counter">Geraakt: 0/17</span>
                    </div>
                    <div class="board-label-group">
                        <span class="board-title">Tracking Bord</span>
                        <span id="hitsMade" class="hit-counter">Treffers: 0/17</span>
                    </div>
                </div>
                <div id="boards">
                    <div id="yourBoard" class="board"></div>
                    <div id="trackingBoard" class="board"></div>
                </div>
            </div>
        `;
        this.initializeBoards();
        this.initializeChat();
    }

    setPlayerInfo(userName) {
        this.userName = userName;
        if (this.connectionReady) {
            this.connection.invoke('RegisterPlayer', this.userName).catch(console.error);
        }
    }

    // Game cell (x=row, y=col) sits at grid index (x+1)*11 + (y+1) in the 11×11 grid
    getGameCell(container, x, y) {
        return container.children[(x + 1) * 11 + (y + 1)];
    }

    initializeBoards() {
        if (this.boardsInitialized) return;

        const rowLabels = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'];
        const yourBoard = this.shadowRoot.getElementById('yourBoard');
        const trackingBoard = this.shadowRoot.getElementById('trackingBoard');

        const buildBoard = (container, isTracking) => {
            // Corner cell
            const corner = document.createElement('div');
            corner.className = 'label-cell';
            container.appendChild(corner);

            // Column labels 1–10
            for (let c = 1; c <= 10; c++) {
                const label = document.createElement('div');
                label.className = 'label-cell';
                label.textContent = c;
                container.appendChild(label);
            }

            // Rows
            for (let r = 0; r < 10; r++) {
                const rowLabel = document.createElement('div');
                rowLabel.className = 'label-cell';
                rowLabel.textContent = rowLabels[r];
                container.appendChild(rowLabel);

                for (let c = 0; c < 10; c++) {
                    const cell = document.createElement('div');
                    cell.className = isTracking ? 'cell trackingCell' : 'cell';
                    container.appendChild(cell);

                    if (isTracking) {
                        const x = r, y = c;
                        cell.addEventListener('click', () => {
                            if (this.gameOver) return;
                            if (this.currentTurnPlayerId !== this.userName) {
                                this.addMessage('Systeem', 'Niet jouw beurt.');
                                return;
                            }
                            if (cell.classList.contains('hit') || cell.classList.contains('miss')) return;
                            this.makeMove(x, y);
                        });
                    }
                }
            }
        };

        buildBoard(yourBoard, false);
        buildBoard(trackingBoard, true);

        this.boardsInitialized = true;
    }

    initializeChat() {
        const sendButton = this.shadowRoot.getElementById('sendButton');
        const messageInput = this.shadowRoot.getElementById('messageInput');

        sendButton.addEventListener('click', () => {
            const message = messageInput.value.trim();
            if (message) {
                this.connection.invoke('SendMessage', this.userName, message)
                    .catch(err => console.error(err));
                messageInput.value = '';
            }
        });

        messageInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') sendButton.click();
        });
    }

    addMessage(user, message) {
        const messagesDiv = this.shadowRoot.getElementById('messages');
        const el = document.createElement('div');
        el.textContent = `${user}: ${message}`;
        messagesDiv.appendChild(el);
        messagesDiv.scrollTop = messagesDiv.scrollHeight;
    }

    setBoardState(boardState) {
        const yourBoard = this.shadowRoot.getElementById('yourBoard');
        const trackingBoard = this.shadowRoot.getElementById('trackingBoard');
        const { PlayerBoard, TrackingBoard, CurrentTurnPlayerId } = boardState;

        if (!PlayerBoard || !TrackingBoard) return;

        this.hitsMade = 0;
        this.hitsReceived = 0;

        for (let r = 0; r < 10; r++) {
            for (let c = 0; c < 10; c++) {
                const playerCell = this.getGameCell(yourBoard, r, c);
                const state = PlayerBoard[r][c];
                playerCell.className = 'cell';
                if (state === 1) playerCell.classList.add('ship');
                if (state === 2) { playerCell.classList.add('hit'); this.hitsReceived++; }
                if (state === 3) playerCell.classList.add('miss');

                const trackCell = this.getGameCell(trackingBoard, r, c);
                const tState = TrackingBoard[r][c];
                trackCell.className = 'cell trackingCell';
                if (tState === 2) { trackCell.classList.add('hit'); this.hitsMade++; }
                if (tState === 3) trackCell.classList.add('miss');
            }
        }

        this.updateCounters();

        if (CurrentTurnPlayerId) {
            this.currentTurnPlayerId = CurrentTurnPlayerId;
        }
    }

    updateTrackingCell(x, y, hit) {
        const trackingBoard = this.shadowRoot.getElementById('trackingBoard');
        const cell = this.getGameCell(trackingBoard, x, y);
        if (!cell) return;
        cell.className = 'cell trackingCell';
        cell.classList.add(hit ? 'hit' : 'miss');
        if (hit) { this.hitsMade++; this.updateCounters(); }
    }

    updatePlayerCell(x, y, hit) {
        const yourBoard = this.shadowRoot.getElementById('yourBoard');
        const cell = this.getGameCell(yourBoard, x, y);
        if (!cell) return;
        cell.className = 'cell';
        cell.classList.add(hit ? 'hit' : 'miss');
        if (hit) { this.hitsReceived++; this.updateCounters(); }
    }

    updateCounters() {
        const hitsMadeEl = this.shadowRoot.getElementById('hitsMade');
        const hitsReceivedEl = this.shadowRoot.getElementById('hitsReceived');
        if (hitsMadeEl) hitsMadeEl.textContent = `Treffers: ${this.hitsMade}/17`;
        if (hitsReceivedEl) hitsReceivedEl.textContent = `Geraakt: ${this.hitsReceived}/17`;
    }

    setupSignalR() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/gameHub")
            .build();

        this.connection.start()
            .then(() => {
                this.connectionReady = true;
                if (this.userName) {
                    this.connection.invoke('RegisterPlayer', this.userName).catch(console.error);
                }
            })
            .catch(err => console.error(err));

        this.connection.on('ReceiveMessage', (user, message) => {
            this.addMessage(user, message);
        });

        this.connection.on('PlayerJoined', (userId, playerCount) => {
            this.dispatchEvent(new CustomEvent('playerjoined', { detail: playerCount, bubbles: true }));
        });

        this.connection.on('UpdateCurrentTurn', (playerId) => {
            this.currentTurnPlayerId = playerId;
            this.dispatchEvent(new CustomEvent('turnchanged', { detail: playerId, bubbles: true }));
        });

        this.connection.on('ReceiveMove', (user, x, y, hit) => {
            if (user === this.userName) {
                this.updateTrackingCell(x, y, hit);
            } else {
                this.updatePlayerCell(x, y, hit);
            }
        });

        this.connection.on('GameOver', (winner) => {
            this.gameOver = true;
            this.addMessage('Systeem', winner === this.userName ? '🎉 Jij wint!' : `${winner} wint!`);
            this.dispatchEvent(new CustomEvent('gameover', { detail: winner, bubbles: true }));
        });

        this.connection.on('GameReset', () => {
            this.dispatchEvent(new CustomEvent('gamereset', { bubbles: true }));
        });

        this.connection.on('ReceiveGameState', (gameState) => {
            this.setBoardState(gameState);
        });
    }

    makeMove(x, y) {
        this.connection.invoke('SendMove', this.userName, x, y)
            .catch(err => console.error(err));
    }
}

customElements.define('game-board', GameBoard);
