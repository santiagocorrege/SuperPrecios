$(function () {
    // abrir menú
    $('#btn-open-categorias').on('click', function (e) {
        e.preventDefault();
        $('#categoria-overlay').addClass('open');
    });

    // cerrar menú (botón X o fondo)
    $('#btn-close-categorias, .categoria-overlay__background').on('click', function () {
        $('#categoria-overlay').removeClass('open');
        // limpiar estado
        $('.categoria-menu__item').removeClass('active');
        $('.submenu-panel').removeClass('active');
    });

    // al hacer hover sobre cada categoría raíz
    $('.categoria-menu__sidebar').on('mouseenter', '.categoria-menu__item', function () {
        var id = $(this).data('id');
        // resaltar en sidebar
        $('.categoria-menu__item').removeClass('active');
        $(this).addClass('active');
        // mostrar panel correspondiente
        $('.submenu-panel').removeClass('active');
        $('.submenu-panel[data-parent-id="' + id + '"]').addClass('active');
    });
});
