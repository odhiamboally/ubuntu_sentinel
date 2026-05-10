export function register(dotNetReference) {
    const notify = () => dotNetReference.invokeMethodAsync("NotifyOnlineStatusChanged", navigator.onLine);
    window.addEventListener("online", notify);
    window.addEventListener("offline", notify);
    notify();

    return {
        dispose: () => {
            window.removeEventListener("online", notify);
            window.removeEventListener("offline", notify);
        }
    };
}

export function isOnline() {
    return navigator.onLine;
}
