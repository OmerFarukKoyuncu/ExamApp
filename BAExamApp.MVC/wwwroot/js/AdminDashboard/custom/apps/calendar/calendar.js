document.addEventListener("DOMContentLoaded", function () {
  var calendarEl = document.getElementById("kt_calendar_app");
  var calendar = new FullCalendar.Calendar(calendarEl, {
    headerToolbar: {
      left: "title",
      center: "",
      right: "dayGridMonth,listWeek",
      // Gün görünümü kullanıldığında fazla etkinlik eklenirse eğer hata görüntüde sorunlar oluşuyor . Bu sebeble fullCalendar kütüphanesi'nin listWeek özelliğini kullandım.
    },
    buttonText: {
      listWeek: selectedLanguage === "tr" ? "Gün" : "Day",
    },
    /* Başka diller eklenirse şöyle ekleme yap
        const buttonTexts = {
            tr: 'Selam',
            en: 'List',
            de: 'Liste',
            fr: 'Liste'
        };

        buttonText: {
            listWeek: buttonTexts[selectedLanguage] || 'List'
        },*/
    footerToolbar: {
      left: "",
      center: "",
      right: "prev,next",
    },
    initialView: "dayGridMonth",
    locale: selectedLanguage, // Takvimin dilini ayarlamak için
    dayMaxEvents: 0,
    moreLinkClick: "popover",
    eventMouseEnter: function (event) {
      eventDataTemplate.startDate = event.event.start;
      eventDataTemplate.endDate = event.event.end;
      eventDataTemplate.eventName = event.event.title;
      showPopover(event.el);
    },
    eventMouseLeave: function (info) {
      hidePopover();
    },
    eventClick: function (event) {
      var eventData = event.event.extendedProps;

      // Eğer not ise, modal göster
      if (eventData.type === "note") {
        var noteId = event.event.id; // Notun ID'sini alıyoruz

        // AJAX isteği yaparak notun detaylarını alıyoruz
        fetch("/admin/note/details?id=" + noteId)
          .then((response) => response.json())
          .then((data) => {
            if (data && data.title && data.description && data.id) {
              // Verileri doğru şekilde modal'a yerleştiriyoruz
              document.getElementById("noteTitle").innerText = data.title;
              document.getElementById("noteContent").innerText =
                data.description;
              document.getElementById("btnNoteDelete").onclick = function () {
                fetch("/admin/note/delete?id=" + data.id, {
                  method: "DELETE",
                })
                  .then((responseJson) => {
                    $("#noteDetailModal").modal("hide");
                    location.reload();
                  })
                  .catch((error) => {
                    console.log(error);
                  });
              };

              // Modal'ı doğru şekilde çağırıyoruz
              var modalElement = document.getElementById("noteDetailModal");

              // Modal'ın yüklendiğinden emin olduktan sonra göster
              if (modalElement) {
                var noteDetailModal = new bootstrap.Modal(modalElement);
                noteDetailModal.show();
              } else {
                console.error("Modal element bulunamadı!");
              }
            } else {
              console.error("Geçersiz veri alındı:", data);
            }
          })
          .catch((error) => {
            console.error("Not detayları alınırken hata oluştu:", error);
          });
      }
      // Eğer etkinlikse, detay sayfasına yönlendir
      else if (eventData.type === "event") {
        window.location.href = "/admin/exam/details?id=" + event.event.id;
      }
      hidePopover();
      showModal();
    },
    datesSet: function (info) {
      var cdate = calendar.getDate();
      var month = cdate.getMonth() + 1;
      var year = cdate.getFullYear();

      var notesUrl = "/Admin/Home/GetNotes?year=" + year + "&month=" + month;
      var eventsUrl = "/Admin/Home/GetEvents?year=" + year + "&month=" + month;

      document.getElementById("loading-overlay").classList.remove("d-none");
      calendarEl.style.filter = "blur(3px)";

      // İki farklı veri kaynağından etkinlikleri almak için
      Promise.all([
        fetch(eventsUrl).then((response) => response.json()),
        fetch(notesUrl).then((response) => response.json()),
      ])
        .then(([eventsData, notesData]) => {
          // Takvimdeki mevcut etkinlikleri temizliyoruz
          calendar.removeAllEvents();

          // Etkinlikleri ekliyoruz (mavi ile göster) ve tür bilgisini ekliyoruz
          eventsData.forEach((event) => {
            event.backgroundColor = "blue"; // Mavi renk
            event.borderColor = "blue"; // Mavi kenarlık
            event.textColor = "white"; // Beyaz metin rengi
            event.extendedProps = { type: "event" }; // 'event' türü ekliyoruz
          });
          calendar.addEventSource(eventsData);

          // Notları ekliyoruz (kırmızı ile göster) ve tür bilgisini ekliyoruz
          notesData.forEach((note) => {
            note.backgroundColor = "red"; // Kırmızı renk
            note.borderColor = "red"; // Kırmızı kenarlık
            note.textColor = "white"; // Beyaz metin rengi
            note.extendedProps = { type: "note" }; // 'note' türü ekliyoruz
          });
          calendar.addEventSource(notesData);
        })
        .catch((error) => {
          console.error("Error fetching data:", error);
        })
        .finally(() => {
          document.getElementById("loading-overlay").classList.add("d-none");
          calendarEl.style.filter = "blur(0px)";
        });
    },
    eventContent: function (arg) {
      // Günlük görünümde etkinlikleri gruplama ve metin boyutunu büyütme
      return {
        html: `<div style="font-size: 16px; font-weight: bold;">${arg.event.title}</div>`,
      };
    },
  });

  var card = document.querySelector("#kt_calendar .card");
  var cardHeader = card.querySelector(".card-header");
  var cardBody = card.querySelector(".card-body");

  cardBody.style.display = "block";
  calendar.render();

  cardHeader.style.cursor = "pointer";
  cardHeader.addEventListener("click", function () {
    if (cardBody.style.display === "none") {
      cardBody.style.display = "block";
      calendar.render();
    } else {
      cardBody.style.display = "none";
      calendar.destroy();
    }
  });
});

("use strict");
var calendar, popoverInstance;
var eventDataTemplate = {
  id: "",
  eventName: "",
  eventDescription: "",
  eventLocation: "",
  startDate: "",
  endDate: "",
  allDay: false,
};

var isPopoverShown = false;

const showPopover = (event) => {
  hidePopover();
  const startDateFormatted = eventDataTemplate.startDate
    ? eventDataTemplate.allDay
      ? moment(eventDataTemplate.startDate).format("Do MMM, YYYY")
      : moment(eventDataTemplate.startDate).format("Do MMM, YYYY - h:mm a")
    : "Invalid date";

  const endDateFormatted = eventDataTemplate.endDate
    ? eventDataTemplate.allDay
      ? moment(eventDataTemplate.endDate).format("Do MMM, YYYY")
      : moment(eventDataTemplate.endDate).format("Do MMM, YYYY - h:mm a")
    : "No end date"; // Bitiş tarihi yoksa daha anlamlı bir mesaj göster

  var popoverOptions = {
    container: "body",
    trigger: "manual",
    boundary: "window",
    placement: "auto",
    dismiss: true,
    html: true,
    title: " ",
    content: `
            <div class="fw-bolder mb-2 mt-3">${eventDataTemplate.eventName}</div>
            <div class="fs-7"><span class="fw-bold">Start:</span> ${startDateFormatted}</div>
            <div class="fs-7 mb-4"><span class="fw-bold">End:</span> ${endDateFormatted}</div>
        `,
  };

  popoverInstance = KTApp.initBootstrapPopover(event, popoverOptions);
  popoverInstance.show();
  isPopoverShown = true;
};

const hidePopover = () => {
  if (isPopoverShown) {
    popoverInstance.dispose();
    isPopoverShown = false;
  }
};
