// Comportamento global do menu lateral.
function abrirMenu() {
    const sidebar = document.getElementById("menuLateral");
    const overlay = document.getElementById("sidebarOverlay");

    if (!sidebar) return;

    sidebar.classList.add("menu-aberto");
    overlay?.classList.add("menu-overlay-aberto");
    document.body.classList.add("menu-mobile-aberto");
}

function fecharMenu() {
    const sidebar = document.getElementById("menuLateral");
    const overlay = document.getElementById("sidebarOverlay");

    sidebar?.classList.remove("menu-aberto");
    overlay?.classList.remove("menu-overlay-aberto");
    document.body.classList.remove("menu-mobile-aberto");
}

document.addEventListener("keydown", function (event) {
    if (event.key === "Escape") fecharMenu();
});

function alternarSubmenu(id, botao) {
    const submenu = document.getElementById(id);

    if (!submenu || !botao) {
        return;
    }

    const estaAberto = botao.getAttribute("aria-expanded") === "true";

    botao.setAttribute("aria-expanded", String(!estaAberto));
    submenu.classList.toggle("submenu-aberto", !estaAberto);
}
