window.addEventListener('message', (event) => {
    if (event.data.type === 'state') {
        if (event.data.text === 'CREATING') {
            $("#race-info").css("display", "block");
        } else {
            $("#race-info").css("display", "none");
        }
    }
    if (event.data.Name || event.data.Name === ''){
        $("#name").html(event.data.Name);
        $("#track").html(event.data.Code);
        $("#car").html(event.data.Car);
        $("#weather").html(event.data.Weather);
        $("#time").html(event.data.Time);
        $("#laps").html(event.data.Laps);
        $("#route").html(event.data.Laps === 0 ? 'Straight' : event.data.Laps === '' ? '' : 'Round');
        $("#checkpoints").html(event.data.Checkpoints.length);
        $("#spawnpoints").html(event.data.Spawnpoints.length);
    }
});