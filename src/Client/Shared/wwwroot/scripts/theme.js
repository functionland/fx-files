const Theme = {
    dark: 'Dark',
    light: 'Light',
    system: 'System'
};

const themeKey = 'theme';
const dataThemeKey = 'data-theme';
const currentTheme = localStorage.getItem(themeKey);

var FxTheme = {

    registerForSystemThemeChanged: (dotnetObj, callbackMethodName) => {
        const media = matchMedia('(prefers-color-scheme: dark)');
        if (media && dotnetObj) {
            media.onchange = args => {
                const isDark = args.matches;
                if (FxTheme.getTheme() = Theme.system) {
                    FxTheme.applyTheme(Theme.system);
                }
                dotnetObj.invokeMethod(callbackMethodName, isDark);
            }
        }
    },

    getSystemTheme: () =>
        matchMedia('(prefers-color-scheme: dark)').matches
            ? Theme.dark : Theme.light,

    getTheme: () => localStorage.getItem(themeKey),

    applyTheme: (theme) => {

        if (theme === Theme.system) {
            theme = FxTheme.getSystemTheme();
        }

        if (theme === Theme.dark) {
            document.body.setAttribute(dataThemeKey, theme);
        } else {
            document.body.removeAttribute(dataThemeKey);
        }
    },

    setTheme: (theme) => {
        FxTheme.applyTheme(theme);
        localStorage.setItem(themeKey, theme);
    }
};

FxTheme.applyTheme(currentTheme);