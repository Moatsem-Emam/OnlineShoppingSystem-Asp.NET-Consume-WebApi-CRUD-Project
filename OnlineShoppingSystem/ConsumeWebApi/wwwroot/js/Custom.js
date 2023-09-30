// custom.js

$(document).ready(function () {
    $('table').on('click', 'button', function () {
        var id = $(this).closest('tr').data('id');
        var currentStatus = $(this).text();
        changeStatus(id, currentStatus);
    });
});

function changeStatus(id, currentStatus) {
    var newStatus = "";

    // Determine the new status based on the current status
    switch (currentStatus.toLowerCase()) {
        case "pending":
            newStatus = "Shipped";
            break;
        case "shipped":
            newStatus = "Arrived";
            break;
        case "arrived":
            newStatus = "Pending";
            break;
        default:
            // Handle other cases or invalid statuses
            break;
    }

    // Send an AJAX request to update the status
    $.ajax({
        url: "https://localhost:7037/api/Orders/ChangeStatus",
        type: "POST",
        data: { id: id, newStatus: newStatus },
        success: function () {
            // Update the button text to the new status
            var button = $('tr[data-id="' + id + '"]').find('button');
            button.text(newStatus);
        },
        error: function (xhr, textStatus, errorThrown) {
            console.error('An error occurred while changing the status:', errorThrown);
            alert('An error occurred while changing the status. Please try again.', errorThrown);
        }
    });
}
