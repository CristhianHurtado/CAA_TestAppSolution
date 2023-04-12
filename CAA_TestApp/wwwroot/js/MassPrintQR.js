$(document).ready(function () {
    $('#print-button').click(function () {
        var qrCodeHtml = $('#qrMass').html();
        var printWindow = window.open('', '', 'height=400,width=800');
        printWindow.document.write('<html><head><title>QR Code Images</title>');
        printWindow.document.write('<style>body { margin: 0; } #qrMass { display: flex; flex-wrap: wrap; justify-content: center; align-items: center; height: 100%; } #qrMass > div { flex: 0 0 20%; margin: 10px; text-align: center; } #qrMass img { max-width: 100%; height: auto; } #qrMass h3 { font-size: 14px; } @media print { #qrMass { display: flex; flex-wrap: wrap; justify-content: center; align-items: center; height: 100%; } #qrMass > div { flex: 0 0 20%; margin: 10px; text-align: center; } }}</style>');
        printWindow.document.write('</head><body>');
        printWindow.document.write(qrCodeHtml);
        printWindow.document.write('</body></html>');
        printWindow.document.close();
        printWindow.focus();
        printWindow.print();
        printWindow.close();
    });
});
