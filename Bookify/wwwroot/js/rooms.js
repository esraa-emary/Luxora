
    const rooms = [
    {
        id: 1,
    number: "301",
    type: "deluxe",
    price: 299,
    status: "available",
    capacity: 2,
    size: 45,
    bedType: "King Size",
    view: "city",
    amenities: ["WiFi", "TV", "Air Conditioning", "Mini Bar"],
    featured: false,
    premium: false,
    image: "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1170&q=80",
    description: "A comfortable deluxe room with modern amenities and a beautiful city view."
            },
    {
        id: 2,
    number: "402",
    type: "executive",
    price: 399,
    status: "occupied",
    capacity: 3,
    size: 60,
    bedType: "King Size × 1, Single × 1",
    view: "ocean",
    amenities: ["WiFi", "TV", "Air Conditioning", "Mini Bar", "Balcony"],
    featured: false,
    premium: true,
    image: "https://images.unsplash.com/photo-1566665797739-1674de7a421a?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1074&q=80",
    description: "Spacious executive suite with ocean view and a private balcony."
            },
    {
        id: 3,
    number: "501",
    type: "suite",
    price: 499,
    status: "available",
    capacity: 4,
    size: 75,
    bedType: "King Size × 2",
    view: "ocean",
    amenities: ["WiFi", "TV", "Air Conditioning", "Mini Bar", "Balcony", "Jacuzzi"],
    featured: true,
    premium: false,
    image: "https://images.unsplash.com/photo-1590490360182-c33d57733427?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1170&q=80",
    description: "Luxurious suite with separate living area and stunning ocean views."
            },
    {
        id: 4,
    number: "601",
    type: "presidential",
    price: 899,
    status: "available",
    capacity: 4,
    size: 120,
    bedType: "King Size × 2",
    view: "city",
    amenities: ["WiFi", "TV", "Air Conditioning", "Mini Bar", "Balcony", "Jacuzzi", "Room Service", "Safe"],
    featured: true,
    premium: true,
    image: "https://images.unsplash.com/photo-1611892440504-42a792e24d32?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1170&q=80",
    description: "The ultimate luxury experience with panoramic city views and exclusive amenities."
            },
    {
        id: 5,
    number: "201",
    type: "deluxe",
    price: 279,
    status: "cleaning",
    capacity: 2,
    size: 40,
    bedType: "Queen Size",
    view: "garden",
    amenities: ["WiFi", "TV", "Air Conditioning"],
    featured: false,
    premium: false,
    image: "https://images.unsplash.com/photo-1522771739844-6a9f6d5f14af?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1171&q=80",
    description: "Cozy deluxe room with garden view and all essential amenities."
            },
    {
        id: 6,
    number: "801",
    type: "royal",
    price: 1299,
    status: "available",
    capacity: 6,
    size: 180,
    bedType: "King Size × 3",
    view: "ocean",
    amenities: ["WiFi", "TV", "Air Conditioning", "Mini Bar", "Balcony", "Jacuzzi", "Room Service", "Safe", "Butler Service"],
    featured: true,
    premium: true,
    image: "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1170&q=80",
    description: "The epitome of luxury with expansive space, premium furnishings, and exclusive services."
            },
    {
        id: 7,
    number: "702",
    type: "penthouse",
    price: 1599,
    status: "reserved",
    capacity: 8,
    size: 220,
    bedType: "King Size × 3, Queen Size × 1",
    view: "city",
    amenities: ["WiFi", "TV", "Air Conditioning", "Mini Bar", "Balcony", "Jacuzzi", "Room Service", "Safe", "Butler Service", "Private Gym", "Private Pool"],
    featured: true,
    premium: true,
    image: "https://images.unsplash.com/photo-1564078516393-cf04bd966897?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1074&q=80",
    description: "An exclusive penthouse with private amenities and breathtaking city views."
            },
    {
        id: 8,
    number: "302",
    type: "deluxe",
    price: 289,
    status: "available",
    capacity: 2,
    size: 42,
    bedType: "King Size",
    view: "city",
    amenities: ["WiFi", "TV", "Air Conditioning", "Mini Bar"],
    featured: false,
    premium: false,
    image: "https://images.unsplash.com/photo-1586105251261-72a756497a11?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1074&q=80",
    description: "Modern deluxe room with comfortable furnishings and city view."
            }
    ];

    const roomsGrid = document.getElementById('roomsGrid');
    const searchInput = document.getElementById('searchInput');
    const viewButtons = document.querySelectorAll('.view-btn');
    const filterChips = document.querySelectorAll('.filter-chip');
    const roomModal = document.getElementById('roomModal');
    const closeModal = document.getElementById('closeModal');
    const clearFiltersBtn = document.getElementById('clearFilters');
    const applyFiltersBtn = document.getElementById('applyFilters');
    const sortSelect = document.getElementById('sortSelect');
    const toast = document.getElementById('toast');
    const toastMessage = document.getElementById('toastMessage');
 
    function renderRooms(roomsToRender = rooms) {
        roomsGrid.innerHTML = '';

    if (roomsToRender.length === 0) {
        roomsGrid.innerHTML = `
                    <div style="grid-column: 1 / -1; text-align: center; padding: 60px 20px; color: var(--text-muted);">
                        <i class="fas fa-search" style="font-size: 3rem; margin-bottom: 20px; opacity: 0.5;"></i>
                        <h3 style="font-weight: 400; margin-bottom: 10px;">No rooms found</h3>
                        <p>Try adjusting your filters or search criteria</p>
                    </div>
                `;
    return;
            }
            
            roomsToRender.forEach(room => {
                const roomCard = document.createElement('div');
    roomCard.className = `room-card ${room.featured ? 'featured' : ''} ${room.premium ? 'premium' : ''}`;

    roomCard.innerHTML = `
    <div class="room-image">
        <img src="${room.image}" alt="Room ${room.number}">
            <div class="room-status status-${room.status}">${room.status}</div>
    </div>
    <div class="room-content">
        <div class="room-header">
            <h3 class="room-title">Room ${room.number}</h3>
            <div class="room-price">$${room.price}<span>/night</span></div>
        </div>
        <div class="room-type">${room.type.charAt(0).toUpperCase() + room.type.slice(1)}</div>
        <div class="room-specs">
            <div class="room-spec"><i class="fas fa-users"></i> ${room.capacity} Guests</div>
            <div class="room-spec"><i class="fas fa-arrows-alt"></i> ${room.size} m²</div>
            <div class="room-spec"><i class="fas fa-binoculars"></i> ${room.view} View</div>
        </div>
        <div class="room-amenities">
            ${room.amenities.slice(0, 3).map(amenity => `
                                <div class="amenity-tag">${amenity}</div>
                            `).join('')}
            ${room.amenities.length > 3 ? `<div class="amenity-tag">+${room.amenities.length - 3}</div>` : ''}
        </div>
        <div class="room-actions">
            <button class="btn-details" data-room="${room.id}">
                <i class="fas fa-info-circle"></i> Details
            </button>
            <button class="btn-book" data-room="${room.id}" ${room.status !== 'available' ? 'disabled' : ''}>
                <i class="fas fa-concierge-bell"></i> ${room.status === 'available' ? 'Book Now' : room.status.charAt(0).toUpperCase() + room.status.slice(1)}
            </button>
        </div>
    </div>
    `;

    roomsGrid.appendChild(roomCard);
            });

            document.querySelectorAll('.btn-details').forEach(button => {
        button.addEventListener('click', function () {
            const roomId = this.getAttribute('data-room');
            openRoomModal(roomId);
        });
            });
            
            document.querySelectorAll('.btn-book').forEach(button => {
        button.addEventListener('click', function () {
            if (this.disabled) return;
            const roomId = this.getAttribute('data-room');
            bookRoom(roomId);
        });
            });
        }   
    function openRoomModal(roomId) {
            const room = rooms.find(r => r.id == roomId);
    if (!room) return;

    document.getElementById('modalRoomTitle').innerHTML = `<i class="fas fa-info-circle"></i> ${room.type.charAt(0).toUpperCase() + room.type.slice(1)} - Room ${room.number}`;

    const modalContent = document.querySelector('.modal-content');
    modalContent.innerHTML = `
    <div class="modal-image">
        <img src="${room.image}" alt="Room ${room.number}">
    </div>
    <div class="modal-details">
        <div class="detail-group">
            <div class="detail-label">Room Type</div>
            <div class="detail-value">${room.type.charAt(0).toUpperCase() + room.type.slice(1)}</div>
        </div>
        <div class="detail-group">
            <div class="detail-label">Price</div>
            <div class="detail-value">$${room.price} <span style="color: var(--text-muted); font-size: 0.9rem;">/ night</span></div>
        </div>
        <div class="detail-group">
            <div class="detail-label">Capacity</div>
            <div class="detail-value">${room.capacity} Guests</div>
        </div>
        <div class="detail-group">
            <div class="detail-label">Size</div>
            <div class="detail-value">${room.size} m²</div>
        </div>
        <div class="detail-group">
            <div class="detail-label">Bed Type</div>
            <div class="detail-value">${room.bedType}</div>
        </div>
        <div class="detail-group">
            <div class="detail-label">View</div>
            <div class="detail-value">${room.view.charAt(0).toUpperCase() + room.view.slice(1)} View</div>
        </div>
        <div class="detail-group">
            <div class="detail-label">Status</div>
            <div class="detail-value"><span class="status-${room.status}" style="padding: 6px 12px; border-radius: 8px; font-size: 0.85rem;">${room.status.charAt(0).toUpperCase() + room.status.slice(1)}</span></div>
        </div>
        <div class="detail-group">
            <div class="detail-label">Amenities</div>
            <div class="amenities-grid">
                ${room.amenities.map(amenity => `
                                <div class="amenity-tag">${amenity}</div>
                            `).join('')}
            </div>
        </div>
        <div class="detail-group">
            <div class="detail-label">Description</div>
            <div class="detail-value">${room.description}</div>
        </div>
    </div>
    `;

    const modalActions = document.querySelector('.modal-actions');
    modalActions.innerHTML = `
    <button class="btn-details"><i class="far fa-heart"></i> Add to Favorites</button>
    <button class="btn-book" ${room.status !== 'available' ? 'disabled' : ''}><i class="fas fa-concierge-bell"></i> ${room.status === 'available' ? 'Book This Room' : 'Not Available'}</button>
    `;

    const modalBookBtn = modalActions.querySelector('.btn-book');
    if (modalBookBtn) {
        modalBookBtn.addEventListener('click', () => {
            if (room.status === 'available') {
                bookRoom(roomId);
            }
        });
            }

    roomModal.classList.add('active');
        }

    function bookRoom(roomId) {
            const room = rooms.find(r => r.id == roomId);
    if (!room) return;

    showToast(`Room ${room.number} booked successfully!`);

    console.log(`Booking Room ${room.number} - ${room.type} for $${room.price}/night`);
        }

    function showToast(message) {
        toastMessage.textContent = message;
    toast.classList.add('show');
            
            setTimeout(() => {
        toast.classList.remove('show');
            }, 3000);
        }

    function filterRooms() {
            const typeFilter = document.getElementById('typeFilter').value;
    const minPrice = document.getElementById('minPrice').value;
    const maxPrice = document.getElementById('maxPrice').value;
    const capacityFilter = document.getElementById('capacityFilter').value;
    const statusFilter = document.getElementById('statusFilter').value;
    const searchQuery = searchInput.value.toLowerCase();
            
            let filteredRooms = rooms.filter(room => {

                if (typeFilter && room.type !== typeFilter) return false;


    if (minPrice && room.price < parseInt(minPrice)) return false;
                if (maxPrice && room.price > parseInt(maxPrice)) return false;


    if (capacityFilter && room.capacity < parseInt(capacityFilter)) return false;


    if (statusFilter && room.status !== statusFilter) return false;


    if (searchQuery &&
    !room.number.toLowerCase().includes(searchQuery) &&
    !room.type.toLowerCase().includes(searchQuery) &&
    !room.view.toLowerCase().includes(searchQuery)) {
                    return false;
                }

    return true;
            });


    const sortValue = sortSelect.value;
    if (sortValue === 'price') {
        filteredRooms.sort((a, b) => a.price - b.price);
            } else if (sortValue === 'capacity') {
        filteredRooms.sort((a, b) => a.capacity - b.capacity);
            } else if (sortValue === 'type') {
        filteredRooms.sort((a, b) => a.type.localeCompare(b.type));
            } else {

        filteredRooms.sort((a, b) => parseInt(a.number) - parseInt(b.number));
            }

    renderRooms(filteredRooms);
        }


        viewButtons.forEach(button => {
        button.addEventListener('click', function () {
            viewButtons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');

            const view = this.getAttribute('data-view');
            showToast(`Switched to ${view} view`);
        });
        });

        filterChips.forEach(chip => {
        chip.addEventListener('click', function () {
            this.classList.toggle('active');

            const filterText = this.textContent.trim();
            showToast(`${this.classList.contains('active') ? 'Applied' : 'Removed'} filter: ${filterText}`);
        });
        });

    closeModal.addEventListener('click', function() {
        roomModal.classList.remove('active');
        });

 
    roomModal.addEventListener('click', function(e) {
            if (e.target === roomModal) {
        roomModal.classList.remove('active');
            }
        });


    clearFiltersBtn.addEventListener('click', function() {
        document.getElementById('typeFilter').value = '';
    document.getElementById('minPrice').value = '';
    document.getElementById('maxPrice').value = '';
    document.getElementById('capacityFilter').value = '';
    document.getElementById('statusFilter').value = '';
    searchInput.value = '';
            
            filterChips.forEach(chip => {
                if (chip.textContent.includes('All Rooms')) {
        chip.classList.add('active');
                } else {
        chip.classList.remove('active');
                }
            });

    filterRooms();
    showToast('All filters cleared');
        });

    applyFiltersBtn.addEventListener('click', function() {
        filterRooms();
    showToast('Filters applied successfully');
        });

    document.getElementById('typeFilter').addEventListener('change', filterRooms);
    document.getElementById('minPrice').addEventListener('input', filterRooms);
    document.getElementById('maxPrice').addEventListener('input', filterRooms);
    document.getElementById('capacityFilter').addEventListener('change', filterRooms);
    document.getElementById('statusFilter').addEventListener('change', filterRooms);
    searchInput.addEventListener('input', filterRooms);
    sortSelect.addEventListener('change', filterRooms);
    document.addEventListener('DOMContentLoaded', function() {
        renderRooms();

            setTimeout(() => {
        document.body.style.opacity = '1';
            }, 100);
        });
    document.body.style.opacity = '0';
    document.body.style.transition = 'opacity 0.5s ease-in-out';
