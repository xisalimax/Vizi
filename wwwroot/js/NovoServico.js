const btnPresencial = document.getElementById("btnPresencial");
const btnOnline = document.getElementById("btnOnline");
const tipoServico = document.getElementById("TipoServico");

btnPresencial.addEventListener("click", function () {
    tipoServico.value = "Presencial";

    btnPresencial.classList.add("ativo");
    btnOnline.classList.remove("ativo");
});

btnOnline.addEventListener("click", function () {
    tipoServico.value = "Online";

    btnOnline.classList.add("ativo");
    btnPresencial.classList.remove("ativo");
});