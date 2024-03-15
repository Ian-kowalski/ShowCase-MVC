const template = document.createElement('template');
template.innerHTML = `<div id="submarine-0" class="section frond"></div><div id="submarine-1" class="section"></div><div id="submarine-2" class="section end"></div>`

export default class Submarine extends HTMLElement {
    constructor() {
        super();
    }
    connectedCallback() {
        this.appendChild(template.content.cloneNode(true));
    }
}

customElements.define('submarine-container', Submarine);