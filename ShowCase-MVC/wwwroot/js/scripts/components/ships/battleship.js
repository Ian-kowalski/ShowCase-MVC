const template = document.createElement('template');
template.innerHTML = `<div id="battleship-0" class="section frond"></div><div id="battleship-1" class="section"></div><div id="battleship-2" class="section"></div><div id="battleship-3" class="section end"></div>`;
template.innerHTML.trim();

export default class Battleship extends HTMLElement {
    constructor() {
        super();
    }
    connectedCallback() {
        this.appendChild(template.content.cloneNode(true));
    }
}

customElements.define('battleship-container', Battleship);