

$(() => {

    const modal = new bootstrap.Modal($("#new-simcha-model")[0]);
    $("#new-simcha").on('click', function () {
        modal.show();
    })

})