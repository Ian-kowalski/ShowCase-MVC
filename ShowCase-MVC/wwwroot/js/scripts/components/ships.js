import destroyer from "./ships/destroyer.js"
import submarine from "./ships/submarine.js"
import cruiser from "./ships/cruiser.js"
import battleship from "./ships/battleship.js"
import carrier from "./ships/carrier.js"

const template = document.createElement('template');
template.innerHTML = `
    <destroyer-container class="ship" draggable="true"></destroyer-container>
    <submarine-container class="ship" draggable="true"></submarine-container>
    <cruiser-container class="ship" draggable="true"></cruiser-container>
    <battleship-container class="ship" draggable="true"></battleship-container>
    <carrier-container class="ship" draggable="true"></carrier-container>
`

export default class Ships extends HTMLElement {


    constructor() {
        super();
    }
    connectedCallback() {
        this.appendChild(template.content.cloneNode(true));
        this.attachStyling();
    }

    attachStyling()
    {
        const link = document.createElement("link");
        link.setAttribute('rel', 'stylesheet');
        link.setAttribute('href', '/css/ship.css');
        //link.setAttribute('asp-append-version', 'true');
        this.appendChild(link);
    }
}

customElements.define('ship-container', Ships);