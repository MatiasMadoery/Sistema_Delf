$(document).ready(function () {
    $("#seleccionImg").change(function () {
        var tam = this.files[0].size;
        if (tam > 5000000) {
            alert("Archivo demasiado grande!")

        } else {
            readURl(this);
        }
    });
});

function readURl(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $("#imagen").attr("src", e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}
