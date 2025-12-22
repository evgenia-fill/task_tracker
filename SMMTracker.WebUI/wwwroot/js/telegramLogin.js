window.telegramLogin = {
    renderWidget: function (elementId, dotNetMethodName) {
        window.onTelegramAuth = (user) => {
<<<<<<< HEAD:BlazorApp1/wwwroot/js/telegramLogin.js
            DotNet.invokeMethodAsync('BlazorApp1', dotNetMethodName, user);
=======
            DotNet.invokeMethodAsync('SMMTracker.WebUI', dotNetMethodName, user);
>>>>>>> masha:src/SMMTracker.WebUI/wwwroot/js/telegramLogin.js
        };
    
        const script = document.createElement('script');
        script.src = 'https://telegram.org/js/telegram-widget.js?22';
        script.async = true;
        
        script.setAttribute('data-telegram-login', 'TrackerSmmBot');

        script.setAttribute('data-size', 'large');
        script.setAttribute('data-onauth', 'onTelegramAuth(user)');
        script.setAttribute('data-request-access', 'write');

        document.getElementById(elementId).appendChild(script);
    }
};