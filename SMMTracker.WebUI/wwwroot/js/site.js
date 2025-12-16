// Мы оставим функции глобальными (5, 6), чтобы они были доступны из onclick в HTML.

let currentCalendarId = 0;
let currentMonth;
let currentYear;
let calendarContainer; // Объявляем, но не ищем!

// 3. Функция для загрузки событий (не зависит от DOM, можно оставить снаружи)
/*
async function fetchEvents(month, year) {
    if (currentCalendarId === 0) {
        return [];
    }

    const url = `/api/calendars/${currentCalendarId}/events?month=${month}&year=${year}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return await response.json();
    } catch (error) {
        console.error("Error fetching events:", error);
        return [];
    }
}
*/

// 4. Основная функция отрисовки календаря
/*
async function renderCalendar() {
    // Критическая проверка: если контейнер не найден, выйти, чтобы избежать ошибки
    if (!calendarContainer) {
        console.error("Контейнер календаря #calendar-container не найден.");
        return;
    }

    // Если ID = 0, показываем кнопку "Создать"
    if (currentCalendarId === 0) {
        calendarContainer.innerHTML = `
            <div class="alert alert-warning text-center">
                <h3>Календарь не найден</h3>
                <p>У вас еще нет активного календаря для вашей команды.</p>
                <button id="createCalendarBtn" class="btn btn-primary mt-3">Создать новый календарь</button>
            </div>
        `;
        document.getElementById('createCalendarBtn').addEventListener('click', handleCreateCalendar);
        return;
    }

    // Инициализация (если еще не инициализировано)
    if (!currentMonth) {
        const now = new Date();
        currentMonth = now.getMonth() + 1; // JS месяцы 0-11, API 1-12
        currentYear = now.getFullYear();
    }

    const events = await fetchEvents(currentMonth, currentYear);

    // Демонстрационная отрисовка:
    calendarContainer.innerHTML = `
        <div class="calendar-header-row">
            <button onclick="changeMonth(-1)">&#9664; Предыдущий</button>
            <h2>${currentMonth}/${currentYear}</h2>
            <button onclick="changeMonth(1)">Следующий &#9654;</button>
        </div>
        <div class="calendar-grid-demo p-3 border">
            <p>Календарь успешно загружен! ID: ${currentCalendarId}</p>
            <p>Найдено событий: ${events.length}</p>
        </div>
    `;
}
*/

// 5. Функция для смены месяца (не зависит от DOM)
/*
function changeMonth(delta) {
    currentMonth += delta;
    if (currentMonth > 12) {
        currentMonth = 1;
        currentYear++;
    } else if (currentMonth < 1) {
        currentMonth = 12;
        currentYear--;
    }
    renderCalendar();
}
*/

// 6. Обработчик создания нового календаря (для ID = 0) (не зависит от DOM)
/*
async function handleCreateCalendar() {
    alert('Функция создания календаря еще не реализована.');
}
*/


// 7. ЗАПУСК: Ищем элементы только после загрузки всего HTML
/*
document.addEventListener('DOMContentLoaded', () => {
    // 1. Получаем ID календаря
    const calendarIdElement = document.getElementById('calendar-container');
    if (calendarIdElement) {
        const id = calendarIdElement.dataset.calendarid;
        currentCalendarId = parseInt(id) || 0;
    }

    // 2. Определяем глобальный контейнер
    calendarContainer = document.getElementById('calendar-container');

    // 3. Запускаем отрисовку
    renderCalendar();
});
*/