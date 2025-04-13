window.scrollElementToBottom = (elementId) => {
    const element = document.getElementById(elementId);
    console.log(element);
    if (element) {
        console.log(element.scrollTop);
        console.log(element.scrollHeight);
        element.scrollTop = element.scrollHeight;
    }
}; 
