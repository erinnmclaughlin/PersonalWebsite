// app.js - General purpose JavaScript functions

// Function to scroll an element to the bottom
window.scrollElementToBottom = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

// Function to get the current theme from localStorage or system preference
window.getCurrentTheme = () => {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme) {
        return savedTheme;
    }
    return 'system';
};

// Function to set theme with specific value (light/dark/system)
window.setTheme = (theme) => {
    if (theme === 'system') {
        localStorage.setItem('theme', 'system');
        window.applySystemTheme();
    } else {
        localStorage.setItem('theme', theme);
        document.documentElement.setAttribute('data-theme', theme);
        window.updateDesktopWallpaper(theme);
    }
};

// Function to toggle theme (light/dark)
window.toggleTheme = () => {
    const htmlElement = document.documentElement;
    const currentTheme = htmlElement.getAttribute('data-theme');
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    
    htmlElement.setAttribute('data-theme', newTheme);
    localStorage.setItem('theme', newTheme);
    window.updateDesktopWallpaper(newTheme);
};

// Apply theme based on system preference
window.applySystemTheme = () => {
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const theme = prefersDark ? 'dark' : 'light';
    document.documentElement.setAttribute('data-theme', theme);
    window.updateDesktopWallpaper(theme);
};

// Function to update desktop wallpaper based on theme
window.updateDesktopWallpaper = (theme) => {
    const desktopElements = document.querySelectorAll('.desktop');
    if (desktopElements && desktopElements.length > 0) {
        desktopElements.forEach(element => {
            if (theme === 'light') {
                element.style.backgroundImage = "url('desktop_wallpaper_light.png')";
            } else {
                element.style.backgroundImage = "url('desktop_wallpaper_dark.png')";
            }
        });
    }
};

// Function to initialize the theme from localStorage or system preference
window.initializeTheme = () => {
    const savedTheme = localStorage.getItem('theme');
    
    if (savedTheme) {
        if (savedTheme === 'system') {
            window.applySystemTheme();
        } else {
            document.documentElement.setAttribute('data-theme', savedTheme);
            window.updateDesktopWallpaper(savedTheme);
        }
    } else {
        window.applySystemTheme();
    }
};

// Add event listener for system theme changes
window.listenForSystemThemeChanges = () => {
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
        const savedTheme = localStorage.getItem('theme');
        if (!savedTheme || savedTheme === 'system') {
            const theme = e.matches ? 'dark' : 'light';
            document.documentElement.setAttribute('data-theme', theme);
            window.updateDesktopWallpaper(theme);
        }
    });
};

// Initialize theme on page load
document.addEventListener('DOMContentLoaded', () => {
    window.initializeTheme();
    window.listenForSystemThemeChanges();
}); 
