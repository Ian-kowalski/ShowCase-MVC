const template = document.createElement('template');
template.innerHTML = `<div id="cruiser-0" class="section frond"></div><div id="cruiser-1" class="section"></div><div id="cruiser-2" class="section end"></div>`

export default class Cruiser extends HTMLElement {
    constructor() {
        super();
    }
    connectedCallback() {
        this.appendChild(template.content.cloneNode(true));
    }
}

customElements.define('cruiser-container', Cruiser);