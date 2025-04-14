// window.js
let dragInfo = {
    element: null,
    dotNetInstance: null,
    isDragging: false,
    isResizing: false,
    startX: 0,
    startY: 0,
    startWidth: 0,
    startHeight: 0,
    startLeft: 0,
    startTop: 0
};

export function initializeWindow(element, dotNetRef) {
    dragInfo.element = element;
    dragInfo.dotNetInstance = dotNetRef;
    
    // Add global mouse event listeners
    document.addEventListener('mousemove', handleMouseMove);
    document.addEventListener('mouseup', handleMouseUp);
    
    // Make sure the window is not dragged outside the viewport
    window.addEventListener('resize', ensureWindowIsVisible);
}

export function startDrag(clientX, clientY) {
    dragInfo.isDragging = true;
    dragInfo.startX = clientX;
    dragInfo.startY = clientY;
    dragInfo.startLeft = dragInfo.element.offsetLeft;
    dragInfo.startTop = dragInfo.element.offsetTop;
    
    // Set focus on the window being dragged
    dragInfo.element.focus();
}

export function startResize(clientX, clientY) {
    dragInfo.isResizing = true;
    dragInfo.startX = clientX;
    dragInfo.startY = clientY;
    dragInfo.startWidth = dragInfo.element.offsetWidth;
    dragInfo.startHeight = dragInfo.element.offsetHeight;
    
    // Set focus on the window being resized
    dragInfo.element.focus();
}

function handleMouseMove(e) {
    if (dragInfo.isDragging) {
        const dx = e.clientX - dragInfo.startX;
        const dy = e.clientY - dragInfo.startY;
        
        const newLeft = dragInfo.startLeft + dx;
        const newTop = dragInfo.startTop + dy;
        
        const width = dragInfo.element.offsetWidth;
        const height = dragInfo.element.offsetHeight;
        
        updatePosition(newLeft, newTop, width, height);
    } else if (dragInfo.isResizing) {
        const dx = e.clientX - dragInfo.startX;
        const dy = e.clientY - dragInfo.startY;
        
        const newWidth = Math.max(300, dragInfo.startWidth + dx);
        const newHeight = Math.max(200, dragInfo.startHeight + dy);
        
        const left = dragInfo.element.offsetLeft;
        const top = dragInfo.element.offsetTop;
        
        updatePosition(left, top, newWidth, newHeight);
    }
}

function handleMouseUp() {
    dragInfo.isDragging = false;
    dragInfo.isResizing = false;
}

function updatePosition(left, top, width, height) {
    // Keep window within screen bounds
    const screenWidth = window.innerWidth;
    const screenHeight = window.innerHeight;
    
    // Ensure a minimum part of the window is always visible
    const minVisible = 50;
    
    left = Math.max(-width + minVisible, Math.min(left, screenWidth - minVisible));
    top = Math.max(0, Math.min(top, screenHeight - minVisible));
    
    // Update component via .NET
    dragInfo.dotNetInstance.invokeMethodAsync('UpdatePosition', left, top, width, height);
}

function ensureWindowIsVisible() {
    if (!dragInfo.element) return;
    
    const rect = dragInfo.element.getBoundingClientRect();
    const minVisible = 50;
    
    if (rect.right < minVisible || rect.bottom < minVisible ||
        rect.left > window.innerWidth - minVisible || rect.top > window.innerHeight - minVisible) {
        // If window is off-screen, reset it to a visible position
        updatePosition(50, 50, rect.width, rect.height);
    }
} 