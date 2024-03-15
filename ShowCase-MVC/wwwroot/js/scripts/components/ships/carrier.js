const template = document.createElement('template');
template.innerHTML = `<div id="carrier-0" class="section frond"></div><div id="carrier-1" class="section"></div><div id="carrier-2" class="section"></div><div id="carrier-3" class="section"></div><div id="carrier-4" class="section end"></div>`

export default class Carrier extends HTMLElement {
    constructor() {
        super();
    }
    connectedCallback() {
        this.appendChild(template.content.cloneNode(true));
    }
}

customElements.define('carrier-container', Carrier);