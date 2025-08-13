export function InitializeBackToTopButton()
{
    const backToTopButton = document.getElementById('backToTop');
    if (backToTopButton !== null) {
        backToTopButton.addEventListener('click', () => {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }
}

export function WatchScroll()
{
    window.addEventListener('scroll', () => {
            if (window.pageYOffset > 100) {
                backToTop.classList.remove('hidden');
            } else {
                backToTop.classList.add('hidden');
            }
        });
}

export function SetCurrentYear()
{
    document.getElementById('year').textContent = new Date().getFullYear();
}

