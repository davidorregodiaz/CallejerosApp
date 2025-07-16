export function registerClickAway(dropdown, navbarReference) {
    function handler(e) {
        if (dropdown != null && !dropdown.contains(e.target)) {
            navbarReference.invokeMethodAsync('CloseMenu');
        }
    }
    document.addEventListener('mousedown', handler);
}