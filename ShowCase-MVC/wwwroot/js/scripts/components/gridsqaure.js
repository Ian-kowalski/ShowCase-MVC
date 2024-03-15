const template = document.createElement('template');
template.setAttribute("id", "gridsqaure");
template.innerHTML = `<div class="gridsqaure"></div>`

export default class GridSqaure extends HTMLElement {

    constructor() {
        super();
        
    }

    connectedCallback() {
        this.appendChild(template.content.cloneNode(true));
        this.addEventListener("click", this.clickOnSqaure)
    }
    
    clickOnSqaure() {
        if(this.querySelector(".gridsqaure")){
            //log(this.querySelector(".gridsqaure").getAttribute("id"));

            var user = document.getElementById("userInput").value;
            var message = this.querySelector(".gridsqaure").getAttribute("id");
            connection.invoke("SendMessage", user, message).catch(function (err) {
                return console.error(err.toString());
            });
            event.preventDefault();
        }
    }
    
}

customElements.define('grid-sqaure', GridSqaure);