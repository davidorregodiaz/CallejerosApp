export function AnimateDogTracks() {
    const dogTracks = document.querySelectorAll('.dog-track');
    dogTracks.forEach(img => img.style.opacity = '0');

    const revealTracks = () => {
        dogTracks.forEach((img, i) => {
            setTimeout(() => {
                img.style.transition = 'opacity 0.5s';
                img.style.opacity = '1';
            }, i * 700); // 500ms delay between each
        });
    };

    const observer = new IntersectionObserver((entries, obs) => {
        if (entries[0].isIntersecting) {
            revealTracks();
            obs.disconnect();
        }
    }, { threshold: 0.3, rootMargin: '50px' });

    if (dogTracks.length > 0) {
        dogTracks.forEach((track) => {
            observer.observe(track);    
        })
    }
}