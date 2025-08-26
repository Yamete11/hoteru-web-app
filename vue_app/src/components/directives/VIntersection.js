export default {
    mounted(el, binding) {
        const options = { root: null, rootMargin: '0px', threshold: 1.0 };

        const handler = typeof binding.value === 'function' ? binding.value : () => {};
        const once = !!(binding.modifiers && binding.modifiers.once);

        const callback = (entries, obs) => {
            if (entries.some(e => e.isIntersecting)) {
                handler(entries, obs);
                if (once) {
                    obs.unobserve(el);
                    obs.disconnect();
                    delete el.__vIntersectionObserver__;
                }
            }
        };

        const observer = new IntersectionObserver(callback, options);
        observer.observe(el);

        el.__vIntersectionObserver__ = observer;
    },

    unmounted(el) {
        const obs = el.__vIntersectionObserver__;
        if (obs) {
            try { obs.unobserve(el); } catch (_) {}
            obs.disconnect();
            delete el.__vIntersectionObserver__;
        }
    }
};
