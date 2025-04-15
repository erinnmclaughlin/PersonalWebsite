export function initializeWindow(element, objRef) {
    return new ResizableWindow(element, objRef);
}

export class ResizableWindow {
    constructor(element, objRef) {
        this.element = element;
        this.objRef = objRef;
        
        this.currentPosX = this.element.offsetLeft;
        this.currentPosY = this.element.offsetTop;
        this.previousPosX = this.element.offsetLeft;
        this.previousPosY = this.element.offsetTop;
        
        this.makeDraggable(this.element);
    }
    
    makeDraggable (element) {
        console.log('making element draggable...');
        const header = element.querySelector('.window-header');
        if (header) {
            console.log('found header!');
            header.onmousedown = this.startDragging.bind(this);
        }
        else {
            console.log('idk where the header is :/');
        }
    }

    startDragging(e) {
        // Get the mouse cursor position and set the initial previous positions to begin
        this.previousPosX = e.clientX;
        this.previousPosY = e.clientY;
        
        // When the mouse is let go, call the closing event
        document.onmouseup = this.stopDragging.bind(this);
        
        // call a function whenever the cursor moves
        document.onmousemove = this.handleDrag.bind(this);
    }

    handleDrag(e) {
        // Calculate the new cursor position by using the previous x and y positions of the mouse
        this.currentPosX = this.previousPosX - e.clientX;
        this.currentPosY = this.previousPosY - e.clientY;
        
        // Replace the previous positions with the new x and y positions of the mouse
        this.previousPosX = e.clientX;
        this.previousPosY = e.clientY;
        
        // Set the element's new position
        this.element.style.top = (this.element.offsetTop - this.currentPosY) + 'px';
        this.element.style.left = (this.element.offsetLeft - this.currentPosX) + 'px';
    }

    stopDragging() {
        document.onmouseup = null;
        document.onmousemove = null;
    }
}
