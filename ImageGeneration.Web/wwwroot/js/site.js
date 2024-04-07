$('#ddlModels').on('change', function () {
    if (this.value == 'dall-e-2') {
        $('#dallE3ImgSize').addClass('d-none');
        $('#dallE2ImgSize').removeClass('d-none');
        $('#dallE3ImgQuality').addClass('d-none');
    }

    if (this.value == 'dall-e-3') {
        $('#dallE3ImgSize').removeClass('d-none');
        $('#dallE2ImgSize').addClass('d-none');
        $('#dallE3ImgQuality').removeClass('d-none');
    }
});

function GenerateImg() {
    event.preventDefault();
    $('#genStatus').removeClass('d-none');
    setTimeout(function () { $("#frmGenImages").submit(); },500);
    
}