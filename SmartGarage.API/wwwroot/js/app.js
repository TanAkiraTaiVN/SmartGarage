const API = '';
let token = localStorage.getItem('sg_token');
let currentUser = null;

async function api(path, method = 'GET', body = null) {
    const headers = { 'Content-Type': 'application/json' };
    if (token) headers['Authorization'] = `Bearer ${token}`;
    const opts = { method, headers };
    if (body) opts.body = JSON.stringify(body);
    const res = await fetch(`${API}/api${path}`, opts);
    const data = await res.json();
    if (!res.ok && res.status === 401) { logout(); throw new Error('Unauthorized'); }
    return data;
}

function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN').format(amount) + ' VND';
}

function formatDate(dateStr) {
    if (!dateStr) return '-';
    return new Date(dateStr).toLocaleString('vi-VN');
}

function statusBadge(status) {
    const map = {
        'Active': ['badge-success', 'Đang gửi'],
        'CheckedOut': ['badge-secondary', 'Đã lấy'],
        'Expired': ['badge-warning', 'Hết hạn'],
        'Cancelled': ['badge-danger', 'Đã hủy'],
        'Available': ['badge-success', 'Trống'],
        'Occupied': ['badge-danger', 'Đã đỗ'],
        'Reserved': ['badge-warning', 'Đã đặt'],
        'Maintenance': ['badge-secondary', 'Bảo trì'],
        'Completed': ['badge-success', 'Hoàn thành'],
        'Pending': ['badge-warning', 'Chờ xử lý'],
        'Failed': ['badge-danger', 'Thất bại'],
        'Refunded': ['badge-info', 'Hoàn tiền'],
    };
    const [cls, label] = map[status] || ['badge-secondary', status];
    return `<span class="badge ${cls}">${label}</span>`;
}

function vehicleTypeLabel(type) {
    const map = { 'Motorcycle': 'Xe máy', 'Car': 'Ô tô', 'Bicycle': 'Xe đạp', 'Truck': 'Xe tải' };
    return map[type] || type;
}

function paymentMethodLabel(method) {
    const map = { 'Cash': 'Tiền mặt', 'CreditCard': 'Thẻ tín dụng', 'DebitCard': 'Thẻ ghi nợ', 'MoMo': 'MoMo', 'ZaloPay': 'ZaloPay', 'BankTransfer': 'Chuyển khoản' };
    return map[method] || method;
}

// Auth
async function login() {
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;
    try {
        const res = await api('/Auth/login', 'POST', { email, password });
        if (res.success) {
            token = res.data.token;
            currentUser = res.data;
            localStorage.setItem('sg_token', token);
            document.getElementById('loginPage').style.display = 'none';
            document.getElementById('mainLayout').style.display = 'flex';
            document.getElementById('currentUser').textContent = `Xin chào, ${res.data.fullName}`;
            loadDashboard();
        } else {
            document.getElementById('loginAlert').innerHTML = `<div class="alert alert-danger">${res.message}</div>`;
        }
    } catch (e) {
        document.getElementById('loginAlert').innerHTML = `<div class="alert alert-danger">Lỗi kết nối server</div>`;
    }
}

function logout() {
    token = null;
    currentUser = null;
    localStorage.removeItem('sg_token');
    document.getElementById('loginPage').style.display = 'flex';
    document.getElementById('mainLayout').style.display = 'none';
}

// Navigation
function showPage(page) {
    document.querySelectorAll('.page-content').forEach(el => el.style.display = 'none');
    document.querySelectorAll('.nav-item').forEach(el => el.classList.remove('active'));
    document.getElementById(`page-${page}`).style.display = 'block';
    document.querySelector(`[data-page="${page}"]`).classList.add('active');

    switch (page) {
        case 'dashboard': loadDashboard(); break;
        case 'tickets': loadTickets(); break;
        case 'vehicles': loadVehicles(); break;
        case 'spots': loadSpots(); break;
        case 'payments': loadPayments(); break;
        case 'checkin': loadCheckinData(); break;
    }
}

// Dashboard
async function loadDashboard() {
    try {
        const res = await api('/Dashboard');
        if (res.success) {
            const d = res.data;
            document.getElementById('statsGrid').innerHTML = `
                <div class="stat-card">
                    <div class="stat-icon blue"><i class="fas fa-parking"></i></div>
                    <div class="stat-info"><h3>${d.totalSpots}</h3><p>Tổng vị trí đỗ</p></div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon green"><i class="fas fa-check-circle"></i></div>
                    <div class="stat-info"><h3>${d.availableSpots}</h3><p>Vị trí trống</p></div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon red"><i class="fas fa-car"></i></div>
                    <div class="stat-info"><h3>${d.occupiedSpots}</h3><p>Đang sử dụng</p></div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon orange"><i class="fas fa-money-bill-wave"></i></div>
                    <div class="stat-info"><h3>${formatCurrency(d.todayRevenue)}</h3><p>Doanh thu hôm nay</p></div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon cyan"><i class="fas fa-chart-line"></i></div>
                    <div class="stat-info"><h3>${formatCurrency(d.monthlyRevenue)}</h3><p>Doanh thu tháng</p></div>
                </div>
                <div class="stat-card">
                    <div class="stat-icon blue"><i class="fas fa-calendar-alt"></i></div>
                    <div class="stat-info"><h3>${formatCurrency(d.yearlyRevenue)}</h3><p>Doanh thu năm</p></div>
                </div>
            `;
        }
    } catch (e) { console.error(e); }

    try {
        const res = await api('/Tickets/active');
        if (res.success) renderTicketsTable('activeTicketsTable', res.data);
    } catch (e) { console.error(e); }
}

// Tickets
async function loadTickets() {
    try {
        const res = await api('/Tickets');
        if (res.success) renderTicketsTable('allTicketsTable', res.data);
    } catch (e) { console.error(e); }
}

function renderTicketsTable(containerId, tickets) {
    if (!tickets.length) {
        document.getElementById(containerId).innerHTML = '<p style="color:var(--text-muted);padding:1rem">Chưa có vé xe nào.</p>';
        return;
    }
    document.getElementById(containerId).innerHTML = `
        <table>
            <thead><tr>
                <th>Mã vé</th><th>Biển số</th><th>Loại xe</th><th>Vị trí</th>
                <th>Check-in</th><th>Check-out</th><th>Phí</th><th>Trạng thái</th>
            </tr></thead>
            <tbody>${tickets.map(t => `<tr>
                <td><strong>${t.ticketCode}</strong></td>
                <td>${t.vehicleLicensePlate}</td>
                <td>${vehicleTypeLabel(t.vehicleType)}</td>
                <td>${t.parkingSpotCode} (${t.zone}-T${t.floor})</td>
                <td>${formatDate(t.checkInTime)}</td>
                <td>${formatDate(t.checkOutTime)}</td>
                <td>${t.totalAmount ? formatCurrency(t.totalAmount) : '-'}</td>
                <td>${statusBadge(t.status)}</td>
            </tr>`).join('')}</tbody>
        </table>`;
}

// Vehicles
async function loadVehicles() {
    try {
        const res = await api('/Vehicles');
        if (res.success) {
            const v = res.data;
            document.getElementById('vehiclesTable').innerHTML = v.length ? `
                <table>
                    <thead><tr><th>Biển số</th><th>Loại</th><th>Hãng</th><th>Dòng</th><th>Màu</th><th>Chủ sở hữu</th><th>Ngày tạo</th></tr></thead>
                    <tbody>${v.map(x => `<tr>
                        <td><strong>${x.licensePlate}</strong></td>
                        <td>${vehicleTypeLabel(x.vehicleType)}</td>
                        <td>${x.brand}</td><td>${x.model}</td><td>${x.color}</td>
                        <td>${x.ownerName}</td><td>${formatDate(x.createdAt)}</td>
                    </tr>`).join('')}</tbody>
                </table>` : '<p style="color:var(--text-muted);padding:1rem">Chưa có phương tiện nào.</p>';
        }
    } catch (e) { console.error(e); }
}

// Spots
async function loadSpots() {
    try {
        const res = await api('/ParkingSpots');
        if (res.success) {
            const spots = res.data;
            const available = spots.filter(s => s.status === 'Available').length;
            const occupied = spots.filter(s => s.status === 'Occupied').length;
            document.getElementById('spotStats').innerHTML = `
                <div class="stat-card"><div class="stat-icon blue"><i class="fas fa-parking"></i></div>
                    <div class="stat-info"><h3>${spots.length}</h3><p>Tổng vị trí</p></div></div>
                <div class="stat-card"><div class="stat-icon green"><i class="fas fa-check"></i></div>
                    <div class="stat-info"><h3>${available}</h3><p>Đang trống</p></div></div>
                <div class="stat-card"><div class="stat-icon red"><i class="fas fa-times"></i></div>
                    <div class="stat-info"><h3>${occupied}</h3><p>Đã sử dụng</p></div></div>
            `;
            document.getElementById('spotsTable').innerHTML = `
                <table>
                    <thead><tr><th>Mã vị trí</th><th>Khu vực</th><th>Tầng</th><th>Loại xe</th><th>Trạng thái</th></tr></thead>
                    <tbody>${spots.map(s => `<tr>
                        <td><strong>${s.spotCode}</strong></td><td>${s.zone}</td><td>${s.floor}</td>
                        <td>${vehicleTypeLabel(s.spotType)}</td><td>${statusBadge(s.status)}</td>
                    </tr>`).join('')}</tbody>
                </table>`;
        }
    } catch (e) { console.error(e); }
}

// Payments
async function loadPayments() {
    try {
        const res = await api('/Payments');
        if (res.success) {
            const p = res.data;
            document.getElementById('paymentsTable').innerHTML = p.length ? `
                <table>
                    <thead><tr><th>Mã GD</th><th>Mã vé</th><th>Khách hàng</th><th>Biển số</th><th>Số tiền</th><th>Phương thức</th><th>Trạng thái</th><th>Ngày</th></tr></thead>
                    <tbody>${p.map(x => `<tr>
                        <td><strong>${x.transactionCode}</strong></td>
                        <td>${x.ticketCode}</td><td>${x.customerName}</td>
                        <td>${x.vehicleLicensePlate}</td>
                        <td>${formatCurrency(x.amount)}</td>
                        <td>${paymentMethodLabel(x.paymentMethod)}</td>
                        <td>${statusBadge(x.status)}</td>
                        <td>${formatDate(x.createdAt)}</td>
                    </tr>`).join('')}</tbody>
                </table>` : '<p style="color:var(--text-muted);padding:1rem">Chưa có giao dịch nào.</p>';
        }
    } catch (e) { console.error(e); }
}

// Check-in/Check-out
async function loadCheckinData() {
    try {
        const [vehicles, spots] = await Promise.all([
            api('/Vehicles'),
            api('/ParkingSpots/available')
        ]);
        if (vehicles.success) {
            document.getElementById('checkinVehicle').innerHTML =
                '<option value="">-- Chọn phương tiện --</option>' +
                vehicles.data.map(v => `<option value="${v.id}">${v.licensePlate} - ${vehicleTypeLabel(v.vehicleType)}</option>`).join('');
        }
        if (spots.success) {
            document.getElementById('checkinSpot').innerHTML =
                '<option value="">-- Chọn vị trí --</option>' +
                spots.data.map(s => `<option value="${s.id}">${s.spotCode} (${s.zone}-T${s.floor}) - ${vehicleTypeLabel(s.spotType)}</option>`).join('');
        }
    } catch (e) { console.error(e); }
}

async function doCheckIn() {
    const vehicleId = parseInt(document.getElementById('checkinVehicle').value);
    const spotId = parseInt(document.getElementById('checkinSpot').value);
    if (!vehicleId || !spotId) { alert('Vui lòng chọn phương tiện và vị trí đỗ.'); return; }
    try {
        const res = await api('/Tickets/check-in', 'POST', { vehicleId, parkingSpotId: spotId });
        if (res.success) {
            document.getElementById('checkinResult').innerHTML = `
                <div class="alert alert-success">
                    <strong>Check-in thành công!</strong><br>
                    Mã vé: <strong>${res.data.ticketCode}</strong><br>
                    Vị trí: ${res.data.parkingSpotCode}
                </div>
                <div class="qr-display">
                    <img src="data:image/png;base64,${res.data.qrCodeBase64}" alt="QR Code">
                    <p style="margin-top:0.5rem;color:var(--text-muted)">Quét mã QR để check-out</p>
                </div>`;
            loadCheckinData();
        } else {
            document.getElementById('checkinResult').innerHTML = `<div class="alert alert-danger">${res.message}</div>`;
        }
    } catch (e) {
        document.getElementById('checkinResult').innerHTML = `<div class="alert alert-danger">Lỗi kết nối</div>`;
    }
}

async function doCheckOut() {
    const ticketCode = document.getElementById('checkoutCode').value.trim();
    const paymentMethod = parseInt(document.getElementById('checkoutPayment').value);
    if (!ticketCode) { alert('Vui lòng nhập mã vé xe.'); return; }
    try {
        const res = await api('/Tickets/check-out', 'POST', { ticketCode, paymentMethod });
        if (res.success) {
            document.getElementById('checkoutResult').innerHTML = `
                <div class="alert alert-success">
                    <strong>Check-out thành công!</strong><br>
                    Xe: ${res.data.vehicleLicensePlate}<br>
                    Thời gian gửi: ${formatDate(res.data.checkInTime)}<br>
                    Thời gian lấy: ${formatDate(res.data.checkOutTime)}<br>
                    <strong>Tổng phí: ${formatCurrency(res.data.totalAmount)}</strong>
                </div>`;
            document.getElementById('checkoutCode').value = '';
        } else {
            document.getElementById('checkoutResult').innerHTML = `<div class="alert alert-danger">${res.message}</div>`;
        }
    } catch (e) {
        document.getElementById('checkoutResult').innerHTML = `<div class="alert alert-danger">Lỗi kết nối</div>`;
    }
}

// Modals
function showAddVehicleModal() { document.getElementById('addVehicleModal').classList.add('active'); }
function showAddSpotModal() { document.getElementById('addSpotModal').classList.add('active'); }
function closeModal(id) { document.getElementById(id).classList.remove('active'); }

async function addVehicle() {
    const body = {
        licensePlate: document.getElementById('vLicensePlate').value,
        vehicleType: parseInt(document.getElementById('vVehicleType').value),
        brand: document.getElementById('vBrand').value,
        model: document.getElementById('vModel').value,
        color: document.getElementById('vColor').value
    };
    try {
        const res = await api('/Vehicles', 'POST', body);
        if (res.success) { closeModal('addVehicleModal'); loadVehicles(); alert('Thêm phương tiện thành công!'); }
        else alert(res.message);
    } catch (e) { alert('Lỗi kết nối'); }
}

async function addSpot() {
    const body = {
        spotCode: document.getElementById('sSpotCode').value,
        zone: document.getElementById('sZone').value,
        floor: parseInt(document.getElementById('sFloor').value),
        spotType: parseInt(document.getElementById('sSpotType').value)
    };
    try {
        const res = await api('/ParkingSpots', 'POST', body);
        if (res.success) { closeModal('addSpotModal'); loadSpots(); alert('Thêm vị trí thành công!'); }
        else alert(res.message);
    } catch (e) { alert('Lỗi kết nối'); }
}

// Init
if (token) {
    document.getElementById('loginPage').style.display = 'none';
    document.getElementById('mainLayout').style.display = 'flex';
    loadDashboard();
}
