$(document).ready(function () {
    let articulos = [];

    // Cargar artículos desde el servidor
    $.getJSON('/Pedidos/GetArticulos', function (data) {
        articulos = data;

        // Cargar el primer articulo al cargar la pagina
        let initialOptions = '';
        for (let articulo of articulos) {
            initialOptions += `<option value="${articulo.id}">${articulo.descripcion}</option>`;
        }
        $('#articulos-container select').html(initialOptions);
    });

    $('#add-item').click(function () {
        if (articulos.length === 0) {
            alert("No hay artículos disponibles para seleccionar.");
            return;
        }

        let options = '';
        for (let articulo of articulos) {
            options += `<option value="${articulo.id}">${articulo.descripcion}</option>`;
        }

        var newRow = `<tr>
                                        <td>
                                            <select name="articuloIds" class="form-control">${options}</select>
                                        </td>
                                        <td>
                                            <input type="number" name="cantidades" class="form-control" />
                                        </td>
                                        <td>
                                            <button type="button" class="btn btn-danger remove-item">Eliminar</button>
                                        </td>
                                      </tr>`;
        $('#articulos-container').append(newRow);
    });

    $(document).on('click', '.remove-item', function () {
        $(this).closest('tr').remove();
    });
});
