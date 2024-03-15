import GridSqaure from './gridsqaure.js';

const template = document.createElement('template');
template.innerHTML = `
    <div class="gameboard"></div>
`

export default class GameBoard extends HTMLElement {
    gridSize;
    gridsqaure = new GridSqaure()

    constructor() {
        super();
        this.gridSize=12;
    }

    connectedCallback() {
        this.appendChild(template.content.cloneNode(true));
        const gameBoard = this.querySelector('.gameboard');

        let counter =0;

        for (let i = 0; i < this.gridSize; i++) {
            for (let j = 0; j < this.gridSize; j++) {
                this.gridsqaure = document.createElement('grid-sqaure');
                gameBoard.appendChild(this.gridsqaure);
                
                if((i == 0 || i == 11) && (j>0 && j<11)){
                    gameBoard.childNodes[counter].querySelector('.gridsqaure').innerHTML =  "<p class='text'>"+String.fromCharCode(64+j)+"</p>";
                    gameBoard.childNodes[counter].querySelector('.gridsqaure').setAttribute("class", "BoardBorder");
                } else if((j==0 || j == 11) && (i>0 && i<11)){
                    gameBoard.childNodes[counter].querySelector('.gridsqaure').innerHTML =  "<p class='text'>"+i+"</p>";
                    gameBoard.childNodes[counter].querySelector('.gridsqaure').setAttribute("class", "BoardBorder");
                }
                else if((i>0 && i<11) && (j>0 && j<11)){
                    gameBoard.childNodes[counter].querySelector('.gridsqaure').setAttribute("id", i+String.fromCharCode(64+j));
                    gameBoard.childNodes[counter].querySelector('.gridsqaure').innerHTML =  "<p class='hitMarker hidden'>"+ "X" + "</p>";
                }else{
                    gameBoard.childNodes[counter].querySelector('.gridsqaure').setAttribute("class", "BoardBorder");
                }
                
                counter += 1;
            }   
        }
        this.attachStyling();
    }

    attachStyling()
    {
        const link = document.createElement("link");
        link.setAttribute('rel', 'stylesheet');
        link.setAttribute('href', '/css/gameBoard.css');
        //link.setAttribute('asp-append-version', 'true');
        this.appendChild(link);
    
    }
}

customElements.define('game-board', GameBoard);