import GameBoard from './components/gameBoard.js';
import Ships from './components/ships.js';

const template_fase1 = document.createElement('template');
template_fase1.innerHTML = `
    <game-board></game-board>
    <ship-container></ship-container>
`

const template_fase2 = document.createElement('template');
template_fase2.innerHTML = `
    <game-board></game-board>
    <game-board></game-board>
`
let draggedShip;
let shipLengt
let selectedShipSection


class Fase1 extends HTMLElement {
    shadow;
    gameboard = new GameBoard();

    constructor() {
        super(); 
    }

    connectedCallback() {
        this.applyTemplate();
        this.attachStyling();
    }

    applyTemplate() {
        this.shadow = this.attachShadow({ mode: "open" });
        this.shadow.appendChild(template_fase1.content.cloneNode(true));



        Array.from(this.shadow.querySelectorAll('.ship')).forEach(ship => {
            ship.addEventListener("click", this.rotate);
            ship.addEventListener("dragstart", this.dragStart);
            ship.addEventListener('dragend', this.dragEnd);
            ship.addEventListener('mousedown', (e) => {
                selectedShipSection = e.target.id
            })
        });
        
        this.gameboard = this.shadow.querySelectorAll('.gridsqaure');

        Array.from(this.shadow.querySelectorAll('.gridsqaure')).forEach(element => {
            element.addEventListener('dragover', this.dragOver, false);
            element.addEventListener('dragenter', this.dragEnter);
            element.addEventListener('dragleave', this.dragLeave);
            element.addEventListener('drop', (event) => { this.dragDrop(event,this.gameboard) });
        });
    }
    
    rotate(){
        this.classList.toggle("ship-vertical")
        Array.from(this.childNodes).forEach(element => {element.classList.toggle("section-vertical")});

    }

    dragStart(e) {
        draggedShip = e.target;
        shipLengt = draggedShip.childNodes.length;
        e.target.classList.add("dragging");
    }

    dragEnd(e) { e.target.classList.remove("dragging"); }

    dragOver(e) {e.preventDefault()}
    
    dragEnter(e) {
        if (e.target.classList.contains("gridsqaure")) {
            e.target.classList.add("dragover");
        }
    }

    dragLeave(e) {
        if (e.target.classList.contains("gridsqaure")) {
            e.target.classList.remove("dragover");
        }
    }

    dragDrop(event, gameboard) {
        event.preventDefault();

        let shipFrontSection = draggedShip.firstChild.id
        let cellId = event.target.id
        let selectetSectionToFrontDif = parseInt(shipFrontSection.substr(-1)) - parseInt(selectedShipSection.substr(-1));

        let cellIdX = parseInt( cellId.slice(0,-1));
        let cellIdY = cellId.charCodeAt(cellId.length -1)
 
        let elements =[]

        if(draggedShip.classList.contains("ship-vertical")){
            for (let index = 0; index < shipLengt; index++) {
                let cord =(cellIdX + selectetSectionToFrontDif + index) + String.fromCharCode(cellIdY);
                const element = findElementById(gameboard,cord);
                if (element == null) {
                    event.target.classList.remove("dragover");
                    return
                }
                elements.push(element)
            }
        }else{
            for (let index = 0; index < shipLengt; index++) {
                let cord = cellIdX + String.fromCharCode(cellIdY + selectetSectionToFrontDif + index);
                const element = findElementById(gameboard,cord);
                if (element == null) {
                    event.target.classList.remove("dragover");
                    return
                }
                elements.push(element)
            }
        }
        for (let index = 0; index < elements.length; index++) {
                elements[index].classList.remove("gridsqaure")
                elements[index].classList.add("ship")
                elements[index].outerHTML = draggedShip.childNodes[index].outerHTML
                draggedShip.remove()
        }
        
        
        if (event.target.classList.contains("gridsqaure")) {
            event.target.classList.remove("dragover");
        }

        

        function findElementById(nodeList, id) {
            for (let i = 0; i < nodeList.length; i++) {
                if (nodeList[i].id === id) {
                    if(!nodeList[i].classList.contains("gridsqaure")){
                        return null;
                    }
                    return nodeList[i];
                }
            }
            return null; // Return null if element with given ID is not found
        }


    }


    attachStyling()
    {
        const link = document.createElement("link");
        link.setAttribute('rel', 'stylesheet');
        link.setAttribute('href', '/css/app.css');
        //link.setAttribute('asp-append-version', 'true');
        this.shadow.appendChild(link);
    
    }
}

customElements.define('game-fase1', Fase1);

class Fase2 extends HTMLElement {

    shadow;

    constructor() {
        super(); 
    }

    connectedCallback() {
        this.shadow = this.attachShadow({ mode: "open" });
        this.shadow.appendChild(template_fase2.content.cloneNode(true));
        this.attachStyling();
    }

    attachStyling()
    {
        const link = document.createElement("link");
        link.setAttribute('rel', 'stylesheet');
        link.setAttribute('href', 'css/app.css');
        this.shadow.appendChild(link);
    
    }
}

customElements.define('game-fase2', Fase2);