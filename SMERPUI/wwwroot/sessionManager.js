window.sessionManager = {
    get: function (key) {
        const value = sessionStorage.getItem(key);
        return value ? JSON.parse(value) : null;
    },
    set: function (key, value) {
        sessionStorage.setItem(key, JSON.stringify(value));
    },
    remove: function (key) {
        sessionStorage.removeItem(key);
    }
};