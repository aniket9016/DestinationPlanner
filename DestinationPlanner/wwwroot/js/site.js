$(document).ready(function () {
    $(document).on('change', '#CountryId', function () {
        var countryId = $(this).val();
        if (countryId) {
            $.ajax({
                url: '/Destinations/GetCitiesByCountry',
                type: 'GET',
                data: { countryId: countryId },
                success: function (data) {
                    $('#CityId').empty();
                    $('#CityId').append('<option value="">Select City</option>');
                    $.each(data, function (index, city) {
                        $('#CityId').append('<option value="' + city.cityId + '">' + city.cityName + '</option>');
                    });
                },
                error: function () {
                    alert("Error loading cities.");
                }
            });
        } else {
            $('#CityId').empty();
            $('#CityId').append('<option value="">Select City</option>');
        }
    });
    $('#Table').DataTable({
        "paging": true,
        "searching": true,
        "ordering": true,
        "info": true,
        "lengthMenu": [5, 10, 25, 50, 100],
        "language": {
            "search": "Filter records:"
        }
    });
});

showInPopup = (url, title) => {
    $("#form-modal .modal-title").html(title);
    $("#form-modal .modal-body").html('<div class="text-center"><i class="fa fa-spinner fa-spin fa-3x"></i></div>');
    $("#form-modal").modal('show');

    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#form-modal .modal-body').html(res);
        },
        error: function (err) {
            console.log(err);
        }
    });
};

jQueryAjaxPost = form => {
    try {
        $("#form-modal .modal-body").html('<div class="text-center"><i class="fa fa-spinner fa-spin fa-3x"></i></div>');

        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    $('#view-all').html(res.html);
                    $('#form-modal').modal('hide');
                    location.reload();
                } else {
                    $('#form-modal .modal-body').html(res.html);
                }
            },
            error: function (err) {
                console.log(err);
            }
        });

        $('#form-modal').on('hidden.bs.modal', function () {
            $('#form-modal .modal-body').html('');
            $('#form-modal .modal-title').html('');
        });

        return false;
    } catch (ex) {
        console.log(ex);
    }
};

jQueryAjaxDelete = form => {
    if (confirm('Are you sure to delete this destination?')) {
        try {
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $("#form-modal .modal-body").html('<div class="text-center"><i class="fa fa-spinner fa-spin fa-3x"></i></div>');
                },
                success: function (res) {
                    $('#view-all').html(res.html);
                },
                error: function (err) {
                    console.log(err);
                }
            });
        } catch (ex) {
            console.log(ex);
        }
    }
    return false;
};
