const Theme = {
    dark: 'Dark',
    light: 'Light',
    system: 'System'
};
const themeKey = 'theme';
const dataThemeKey = 'data-theme';

export const registerForSystemThemeChanged = (dotnetObj, callbackMethodName) => {
    const media = matchMedia('(prefers-color-scheme: dark)');
    if (media && dotnetObj) {
        media.onchange = args => {
            const isDark = args.matches;
            if (getTheme() = Theme.system) {
                applyTheme(Theme.system);
            }
            dotnetObj.invokeMethod(callbackMethodName, isDark);
        }
    }
}

export const getSystemTheme = () =>
    matchMedia('(prefers-color-scheme: dark)').matches
        ? Theme.dark : Theme.light;

export const getTheme = () => {
    return localStorage.getItem(themeKey);
}

export const applyTheme = (theme) => {

    if (theme === Theme.system) {
        theme = getSystemTheme();
    }

    if (theme === Theme.dark) {
        document.body.setAttribute(dataThemeKey, theme);
    } else {
        document.body.removeAttribute(dataThemeKey);
    }
}

export const setTheme = (theme) => {
    applyTheme(theme);
    localStorage.setItem(themeKey, theme);
}