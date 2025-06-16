$(document).on("click", ".pagina-link", function (e) {
    e.preventDefault();
    const pagina = $(this).data("pagina");

    const queryParams = new URLSearchParams(window.location.search);
    const busqueda = queryParams.get("busqueda");

    const baseUrl = window.location.pathname;
    const newUrl = `${baseUrl}?pagina=${pagina}${busqueda ? `&busqueda=${encodeURIComponent(busqueda)}` : ""}`;
    history.pushState(null, "", newUrl);

    $.ajax({
        url: baseUrl,
        data: { pagina, busqueda },
        success: function (html) {
            $('#contenedor-productos').html(html);
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    });
});
