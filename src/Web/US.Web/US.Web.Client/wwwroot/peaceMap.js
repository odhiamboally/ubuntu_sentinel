const maps = new Map();

export function initializePeaceMap(elementId, zones, dotNetReference) {
    const element = document.getElementById(elementId);
    if (!element) {
        return;
    }

    const existing = maps.get(elementId);
    if (existing) {
        existing.remove();
        maps.delete(elementId);
    }

    if (!window.L) {
        element.innerHTML = "<div class='map-fallback'>Map tiles could not load. Zone intelligence remains available in the side panel.</div>";
        return;
    }

    const map = window.L.map(element, {
        center: [1.5, 22],
        zoom: 4,
        minZoom: 3,
        maxZoom: 8,
        scrollWheelZoom: false
    });

    maps.set(elementId, map);

    window.L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 19,
        attribution: "&copy; OpenStreetMap contributors"
    }).addTo(map);

    const bounds = [];

    zones.forEach((zone, index) => {
        const cssClass = read(zone, "cssClass") === "resilience-zone" ? "resilience-zone" : "conflict-zone";
        const latitude = read(zone, "latitude");
        const longitude = read(zone, "longitude");
        const icon = window.L.divIcon({
            className: `zone-div-icon ${cssClass}`,
            html: `<span>${read(zone, "label")}</span>`,
            iconSize: [34, 34],
            iconAnchor: [17, 17],
            popupAnchor: [0, -18]
        });

        const marker = window.L.marker([latitude, longitude], { icon })
            .addTo(map)
            .bindPopup(`<strong>${escapeHtml(read(zone, "name"))}</strong><br>${escapeHtml(read(zone, "region"))}<br>${escapeHtml(read(zone, "evidence"))}`);

        marker.on("click", () => dotNetReference.invokeMethodAsync("SelectZone", index));
        bounds.push([latitude, longitude]);
    });

    if (bounds.length > 1) {
        map.fitBounds(bounds, { padding: [34, 34], maxZoom: 5 });
    }
}

export function disposePeaceMap(elementId) {
    const existing = maps.get(elementId);
    if (!existing) {
        return;
    }

    existing.remove();
    maps.delete(elementId);
}

function read(source, camelName) {
    const pascalName = `${camelName.charAt(0).toUpperCase()}${camelName.slice(1)}`;
    return source[camelName] ?? source[pascalName];
}

function escapeHtml(value) {
    const div = document.createElement("div");
    div.innerText = value ?? "";
    return div.innerHTML;
}
