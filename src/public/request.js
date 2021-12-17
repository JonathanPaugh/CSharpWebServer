function request(path, data, onComplete) {
    $.ajax({
        url: path,
        method: "POST",
        data: JSON.stringify(data)
    }).done((result) => {
        if (onComplete) {
            onComplete(result);
        }
    });
}