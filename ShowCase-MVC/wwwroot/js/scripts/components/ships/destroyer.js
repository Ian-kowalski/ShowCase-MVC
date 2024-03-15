const template = document.createElement('template');
template.innerHTML = `<div id="destroyer-0" class="frond section"></div><div id="destroyer-1" class="end section"></div>`

export default class Destroyer extends HTMLElement {
    constructor() {
        super();
    }
    connectedCallback() {
        this.appendChild(template.content.cloneNode(true));
    }
}

customElements.define('destroyer-container', Destroyer);