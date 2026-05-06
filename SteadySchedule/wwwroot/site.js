console.log("🔥 SITE JS LOADED");

window.scrollToElement = (id) => {
    const el = document.getElementById(id);
    if (el) {
        el.scrollIntoView({
            behavior: "smooth",
            block: "start"
        });
    }
};

let deferredPrompt = null;

window.setupInstallPrompt = (dotnetHelper) => {
    window.addEventListener('beforeinstallprompt', (e) => {
        e.preventDefault();
        deferredPrompt = e;

        dotnetHelper.invokeMethodAsync('ShowInstallButton');
    });
};

window.installApp = async () => {
    if (!deferredPrompt) return;

    deferredPrompt.prompt();
    const result = await deferredPrompt.userChoice;

    deferredPrompt = null;

    return result.outcome;
};