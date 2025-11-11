$(() => {

    $("#new-contributor").on('click', function () {

        resetContributorForm();
        new bootstrap.Modal($(".new-contrib")[0]).show();
    })

    $(".deposit-button").on('click', function () {
        const id = $(this).data("contribid");

        console.log(id);
        $('[name="contributorId"]').val(id);
        new bootstrap.Modal($(".deposit")[0]).show();
      
    })

    //$(".form-control-sm").on('keyup', function () {
    //    const searchText = $(".form-control-sm").val();
    //    $(".name").filter(p => $(".name").val().indexOf(searchText) < 0).closect("tr").hide();
    //})
    
    //$("#clear").on("click", function () {
    //    console.log("clear");
    //    $(".form-control-sm").val('');
    //})

    $(".edit-contrib").on('click', function () {

        $("#edit-id").remove();

        const id = $(this).data("id");
        const firstName = $(this).data("first-name");
        const lastName = $(this).data("last-name");
        const cell = $(this).data("cell");
        const alwaysInclude = $(this).data("always-include");
        const date = $(this).data("date")

        const form = $(".new-contrib form")

        form.append(`<input type='hidden' id='edit-id' name='id' value='${id}'>`)

        $("#contributor_first_name").val(firstName);
        $("#contributor_last_name").val(lastName);
        $("#contributor_cell_number").val(cell);
        $("#contributor_always_include").prop('checked', alwaysInclude === 'True');
        $("#contributor_created_at").val(date);

        $("#initialDepositDiv").hide();

        new bootstrap.Modal($(".new-contrib")[0]).show();
        $(form).attr('action', "/contributors/edit");
    })

    function resetContributorForm() {

        $("#contributor_first_name").val("");
        $("#contributor_last_name").val("");
        $("#contributor_cell_number").val("");
        $("#contributor_created_at").val("");
        $("#contributer_always_include").prop('checked', true);

        $("#initialDepositDiv").show();
        const form = $(".new-contrib form")
        $(form).attr('action', "/contributors/new");
    }
})