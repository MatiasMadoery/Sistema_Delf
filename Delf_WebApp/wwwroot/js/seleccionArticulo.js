$(document).ready(function () {
    let articulos = [];

    // Cargar artículos desde el servidor
    $.getJSON('/Pedidos/GetArticulos', function (data) {
        articulos = data;
        let options = '';
        for (let articulo of articulos) {
            options += `<option value="${articulo.id}">${articulo.descripcion}</option>`;
        }
        $('#articulos-container select').html(options).select2();
    });

    // Añadir nuevo artículo
    $('#add-item').click(function () {
        if (articulos.length === 0) {
            alert("No hay artículos disponibles para seleccionar.");
            return;
        }

        let options = '';
        for (let articulo of articulos) {
            options += `<option value="${articulo.id}">${articulo.descripcion}</option>`;
        }

        let newRow = `<tr>
                        <td>
                            <select name="articuloIds" class="form-control articulo-select">${options}</select>
                        </td>
                        <td>
                            <input type="number" name="cantidades" class="form-control" />
                        </td>
                        <td>
                            <button type="button" class="btn btn-danger remove-item">Eliminar</button>
                        </td>
                      </tr>`;

        $('#articulos-container').append(newRow);
        $('#articulos-container select').last().select2();
    });

    $(document).on('click', '.remove-item', function () {
        $(this).closest('tr').remove();
    });

    // Establecer la fecha actual en el input de fecha
    var today = new Date().toISOString().split('T')[0];
    $('input[type="date"]').val(today);
});
