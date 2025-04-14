// window.js
// Track all windows that have been initialized
let windowRegistry = [];

// Active drag/resize info
let activeOperation = {
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
    // Add this window to our registry with its reference and .NET instance
    windowRegistry.push({
        element: element,
        dotNetInstance: dotNetRef,
        windowId: element.getAttribute('data-window-id')
    });
    
    // Add event listeners specific to this window
    element.addEventListener('mousedown', (e) => handleWindowMouseDown(e, element, dotNetRef));
    
    // Make sure the window is not dragged outside the viewport
    // This needs to be a global handler
    if (windowRegistry.length === 1) {
        // Only add global handlers once
        document.addEventListener('mousemove', handleMouseMove, { passive: true });
        document.addEventListener('mouseup', handleMouseUp);
        window.addEventListener('resize', ensureWindowsVisible);
    }
}

// Handle mousedown on any window
function handleWindowMouseDown(e, element, dotNetRef) {
    // Get the window ID
    const windowId = element.getAttribute('data-window-id');
    
    // Skip activation if clicking window control buttons
    const isControlClick = e.target.closest('.window-controls');
    if (!isControlClick) {
        // Activate the window first
        const thisIsActive = element.classList.contains('active');
        if (!thisIsActive) {
            dotNetRef.invokeMethodAsync('ActivateWindow', windowId);
        }
    }
    
    // Handle drag if clicking header and not on control buttons
    if (e.target.closest('.window-header') && !isControlClick) {
        startDrag(e, element, dotNetRef);
    }
    
    // Handle resize if clicking resize handle
    if (e.target.closest('.resize-handle')) {
        startResize(e, element, dotNetRef);
    }
}

function startDrag(e, element, dotNetRef) {
    if (activeOperation.isDragging) return; // Prevent multiple drag starts
    
    // Skip dragging for maximized windows
    if (element.classList.contains('maximized')) return;
    
    // Set the current active window for dragging
    activeOperation.element = element;
    activeOperation.dotNetInstance = dotNetRef;
    activeOperation.isDragging = true;
    activeOperation.startX = e.clientX;
    activeOperation.startY = e.clientY;
    activeOperation.startLeft = element.offsetLeft;
    activeOperation.startTop = element.offsetTop;
    
    // Add a class for visual indication
    element.classList.add('dragging');
    
    // Prevent default to avoid text selection
    e.preventDefault();
}

function startResize(e, element, dotNetRef) {
    if (activeOperation.isResizing) return; // Prevent multiple resize starts
    
    // Skip resizing for maximized windows
    if (element.classList.contains('maximized')) return;
    
    // Set the current active window for resizing
    activeOperation.element = element;
    activeOperation.dotNetInstance = dotNetRef;
    activeOperation.isResizing = true;
    activeOperation.startX = e.clientX;
    activeOperation.startY = e.clientY;
    activeOperation.startWidth = element.offsetWidth;
    activeOperation.startHeight = element.offsetHeight;
    
    // Add a class for visual indication
    element.classList.add('resizing');
    
    // Prevent default to avoid text selection
    e.preventDefault();
}

function handleMouseMove(e) {
    if (activeOperation.isDragging) {
        const dx = e.clientX - activeOperation.startX;
        const dy = e.clientY - activeOperation.startY;
        
        const newLeft = boundPosition(activeOperation.startLeft + dx, activeOperation.element.offsetWidth, 'x');
        const newTop = boundPosition(activeOperation.startTop + dy, activeOperation.element.offsetHeight, 'y');
        
        // Direct DOM update - no transitions or transforms
        activeOperation.element.style.left = `${newLeft}px`;
        activeOperation.element.style.top = `${newTop}px`;
    } else if (activeOperation.isResizing) {
        const dx = e.clientX - activeOperation.startX;
        const dy = e.clientY - activeOperation.startY;
        
        const newWidth = Math.max(300, activeOperation.startWidth + dx);
        const newHeight = Math.max(200, activeOperation.startHeight + dy);
        
        // Direct DOM update
        activeOperation.element.style.width = `${newWidth}px`;
        activeOperation.element.style.height = `${newHeight}px`;
    }
}

function handleMouseUp() {
    if (activeOperation.isDragging || activeOperation.isResizing) {
        // Get final position/size
        const left = activeOperation.element.offsetLeft;
        const top = activeOperation.element.offsetTop;
        const width = activeOperation.element.offsetWidth;
        const height = activeOperation.element.offsetHeight;
        
        // Remove drag/resize classes immediately
        activeOperation.element.classList.remove('dragging', 'resizing');
        
        // Create a ref to the active element and instance before resetting
        const activeElement = activeOperation.element;
        const activeDotNetInstance = activeOperation.dotNetInstance;
        
        // Reset flags for immediate UI response
        const wasDragging = activeOperation.isDragging;
        const wasResizing = activeOperation.isResizing;
        activeOperation.isDragging = false;
        activeOperation.isResizing = false;
        activeOperation.element = null;
        
        // Only notify C# after everything is reset in the UI
        if ((wasDragging || wasResizing) && activeDotNetInstance) {
            // Use requestAnimationFrame to ensure DOM updates are complete
            requestAnimationFrame(() => {
                activeDotNetInstance.invokeMethodAsync('UpdatePosition', left, top, width, height);
            });
        }
    }
}

// Bound a position to keep within screen bounds
function boundPosition(position, dimension, axis) {
    const screenDimension = axis === 'x' ? window.innerWidth : window.innerHeight;
    const minVisible = 50;
    
    return Math.max(-dimension + minVisible, Math.min(position, screenDimension - minVisible));
}

function ensureWindowsVisible() {
    windowRegistry.forEach(item => {
        if (!item.element) return;
        
        const rect = item.element.getBoundingClientRect();
        const minVisible = 50;
        
        if (rect.right < minVisible || rect.bottom < minVisible ||
            rect.left > window.innerWidth - minVisible || rect.top > window.innerHeight - minVisible) {
            // If window is off-screen, reset it to a visible position
            const left = 50;
            const top = 50;
            
            // Update position without transitions
            item.element.style.left = `${left}px`;
            item.element.style.top = `${top}px`;
            
            // Notify C#
            item.dotNetInstance.invokeMethodAsync('UpdatePosition', 
                left, top, item.element.offsetWidth, item.element.offsetHeight);
        }
    });
} 