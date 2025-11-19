window.telegramLogin = {
    renderWidget: function (elementId, dotNetMethodName) {
        // Эта функция будет вызвана Telegram после успешной авторизации
        window.onTelegramAuth = (user) => {
            // Вызываем C# метод, имя которого нам передали из Blazor
            DotNet.invokeMethodAsync('SMMTracker.WebUI', dotNetMethodName, user);
        };

        // Создаем и добавляем на страницу тег <script> для виджета Telegram
        const script = document.createElement('script');
        script.src = 'https://telegram.org/js/telegram-widget.js?22';
        script.async = true;

        // ВАЖНО: ЗАМЕНИТЕ НА ИМЯ ВАШЕГО БОТА
        script.setAttribute('data-telegram-login', 'TrackerSmmBot');

        script.setAttribute('data-size', 'large');
        script.setAttribute('data-onauth', 'onTelegramAuth(user)');
        script.setAttribute('data-request-access', 'write');

        document.getElementById(elementId).appendChild(script);
    }
};