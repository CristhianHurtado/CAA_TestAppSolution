$(document).ready(function () {
    $('#print-button').click(function () {
        var qrCodeImage = document.getElementById("qr-code-image");
        var printWindow = window.open('', '', 'height=400,width=800');
        printWindow.document.write('<html><head><title>QR Code Image</title></head><body>');
        printWindow.document.write('<img src="' + qrCodeImage.src + '">');
        printWindow.document.write('</body></html>');
        printWindow.document.close();
        printWindow.focus();
        printWindow.print();
        printWindow.close();
    });
});
