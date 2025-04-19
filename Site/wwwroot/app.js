// This function scrolls the provided element to the bottom
window.scrollElementToBottom = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
}; 