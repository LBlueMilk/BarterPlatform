//Tooltips啟動
const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))

//點擊新增.clicked
document.addEventListener('DOMContentLoaded', function () {
    const section = document.querySelector('section');
    const welcomeMessage = document.querySelector('.welcome-message');

    section.addEventListener('click', function () {
        section.classList.toggle('clicked');
        welcomeMessage.classList.toggle('messageclicked');
    });
});
