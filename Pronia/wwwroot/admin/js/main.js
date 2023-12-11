let deleteBtn = document.querySelectorAll(".item-delete")

deleteBtn.forEach(btn => btn.addEventListener("click", function (a) {
    a.preventDefault();

    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            let url = btn.getAttribute("href")
            fetch(url)
                .then(response => {
                    if (response.status == 200) {
                        Swal.fire({
                            title: "Deleted!",
                            text: "Your file has been deleted.",
                            icon: "success"
                        });
                    }
                    
                    else {
                        Swal.fire({
                            title: "Deleted!",
                            text: "Your file has been deleted.",
                            icon: "error"
                        });
                    }
                })
        }
    });
}))