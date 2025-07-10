//SelectLists
let subjects = [];
let subtopics = [];
let examRuleSubjects = [];
let examRuleSubjectsTable = [];
let questionTypes = [];
let questionDifficulties = [];
let examTypes = [];

onPageLoad();

async function onPageLoad() {

    let examRuleSubjectsJSON = JSON.parse(document.getElementById("examRuleSubjects").value);
    isEditing = !!document.getElementById("examRuleSubjects").value;

    await loadExamTypes();

    await onProductChange();

    // Eğer varsa, verileri yükle
    if (examRuleSubjectsJSON && examRuleSubjectsJSON.length > 0) {
        
        let firstRule = examRuleSubjectsJSON[0];
        
        document.getElementById("ExamType").value = firstRule.ExamType;
        await onExamTypeChange();
    }

    for (examRuleSubject of examRuleSubjectsJSON) {
        await loadExamTypes();
        document.getElementById("ExamType").value = examRuleSubject.ExamType;
        await onExamTypeChange();

        var subtopics = await getSubtopics(examRuleSubject.SubjectId)
        var foundSubtopic = subtopics.find(subtopic => subtopic.value === examRuleSubject.SubtopicId);
        var questionDiff = await getQuestionDifficulties(examRuleSubject.ExamType)
        var foundQuestionDiff = questionDiff.find(questionDiff => questionDiff.value === examRuleSubject.QuestionDifficultyId)
        var examTypes = await getExamTypes();
        var foundExamType = examTypes.find(examType => Number(examType.value) === examRuleSubject.ExamType)
        var questionTypes = await getQuestionTypes(examRuleSubject.ExamType)
        var foundQuestionType = questionTypes.find(questionType => Number(questionType.value) === examRuleSubject.QuestionType)

        let examRuleSubjectVM = {
            SubjectId: examRuleSubject.SubjectId,
            SubtopicId: examRuleSubject.SubtopicId,
            ExamType: examRuleSubject.ExamType,
            QuestionType: examRuleSubject.QuestionType,
            QuestionDifficultyId: examRuleSubject.QuestionDifficultyId,
            QuestionCount: examRuleSubject.QuestionCount,
        }
        examRuleSubjects.push(examRuleSubjectVM);

        examRuleSubjectsTable.push({
            SubjectName: await getSubjectNameBySubjectId(examRuleSubject.SubjectId),
            SubtopicName: foundSubtopic.text,
            ExamType: foundExamType.text,
            QuestionType: foundQuestionType.text,
            QuestionDifficulty: foundQuestionDiff.text,
            QuestionCount: examRuleSubject.QuestionCount
        });
    }

    updateTable();

}

//Product type change event.
async function onProductChange() {

    if (!isEditing) {
        examRuleSubjects = [];
        examRuleSubjectsTable = [];
    }

    subjects = await getSubjects($("#ProductId").val());
    populateSelectList("SubjectId", subjects);

    updateTable();

    document.getElementById("examRuleSubjects").value = JSON.stringify(examRuleSubjects);
}

//Product type change event.
async function onSubjectChange() {
    subtopics = await getSubtopics($("#SubjectId").val());
    populateSelectList("SubtopicId", subtopics);
}

//Exam type change event.
async function onExamTypeChange() {
    questionTypes =await getQuestionTypes($("#ExamType").val());
    questionDifficulties = await getQuestionDifficulties($("#ExamType").val());

    populateSelectList("QuestionType", questionTypes);
    populateSelectList("QuestionDifficultyId", questionDifficulties);
};

let formchanged = false;
let ruleAdded = false;
let ruleDeleted = false;

//Add new rule
async function addNewRule() {
    $("#productValidation").text("");
    $("#subjectValidation").text("");
    $("#subtopicValidation").text("");
    $("#questionDifficultyValidation").text("");
    $("#questionTypeValidation").text("");
    $("#questionCountValidation").text("");

    // Alanların değerlerini alalım
    var productId = $("#ProductId").val();
    var subjectId = $("#SubjectId").val();
    var subtopicId = $("#SubtopicId").val();
    var questionDifficultyId = $("#QuestionDifficultyId").val();
    var questionType = $("#QuestionType").val();
    var questionCount = $("#QuestionCount").val();

    // Formun geçerli olup olmadığını takip etmek için
    var isValid = true;

    if (!productId) {
        $("#productValidation").text(localizedTexts.productValidationText);
        isValid = false;
    }

    if (!subjectId) {
        $("#subjectValidation").text(localizedTexts.subjectValidationText);
        isValid = false;
    }

    if (!subtopicId) {
        $("#subtopicValidation").text(localizedTexts.chooseSubtopicValidationText);
        isValid = false;
    }

    if (!questionDifficultyId) {
        $("#questionDifficultyValidation").text(localizedTexts.questionDifficultyValidationText);
        isValid = false;
    }

    if (!questionType) {
        $("#questionTypeValidation").text(localizedTexts.questionTypeValidationText);
        isValid = false;
    }

    if (!questionCount || questionCount <= 0) {
        $("#questionCountValidation").text(localizedTexts.numberOfQuestionValidationText);
        isValid = false;
    }

    if (!isValid) {
        return;
    }
 
        document.getElementById("inputValidation").setAttribute("hidden", true);
        document.getElementById("examTypeValidation").setAttribute("hidden", true);


        let examRuleSubjectVM = {
            SubjectId: document.getElementById("SubjectId").value,
            SubtopicId: document.getElementById("SubtopicId").value,
            ExamType: parseInt(document.getElementById("ExamType").value),
            QuestionType: parseInt(document.getElementById("QuestionType").value),
            QuestionDifficultyId: document.getElementById("QuestionDifficultyId").value,
            QuestionCount: parseInt(document.getElementById("QuestionCount").value),
        }

        let hasSameExamType = examRuleSubjects.some((rule) => {
            return (rule.ExamType === examRuleSubjectVM.ExamType);
        });

        if ((hasSameExamType && examRuleSubjects.length > 0) || examRuleSubjects.length === 0) {
            let hasSameRule = examRuleSubjects.some((rule) => {
                return (rule.SubjectId === examRuleSubjectVM.SubjectId
                    && rule.SubtopicId === examRuleSubjectVM.SubtopicId
                    && rule.ExamType === examRuleSubjectVM.ExamType
                    && rule.QuestionType === examRuleSubjectVM.QuestionType
                    && rule.QuestionDifficultyId === examRuleSubjectVM.QuestionDifficultyId);
            });

            if (!hasSameRule) {
                document.getElementById("sameRuleValidation").setAttribute("hidden", true);
                examRuleSubjects.push(examRuleSubjectVM);

                examRuleSubjectsTable.push({
                    SubjectName: document.getElementById("SubjectId").options[document.getElementById("SubjectId").selectedIndex].text,
                    SubtopicName: document.getElementById("SubtopicId").options[document.getElementById("SubtopicId").selectedIndex].text,
                    ExamType: document.getElementById("ExamType").options[document.getElementById("ExamType").selectedIndex].text,
                    QuestionType: document.getElementById("QuestionType").options[document.getElementById("QuestionType").selectedIndex].text,
                    QuestionDifficulty: document.getElementById("QuestionDifficultyId").options[document.getElementById("QuestionDifficultyId").selectedIndex].text,
                    QuestionCount: parseInt(document.getElementById("QuestionCount").value)
                })
                refreshModal();
                ruleAdded = true;
                enableUpdateButton();
            }
            else {
                document.getElementById("sameRuleValidation").removeAttribute("hidden");
            }
            refreshModal();
        }
        else {
            document.getElementById("examTypeValidation").removeAttribute("hidden");
        }
    return false;
}

function enableUpdateButton() {
    const updateButton = document.querySelector("#updateButton");
    
    if (ruleAdded || ruleDeleted || formChanged) {
        updateButton.disabled = false;  
    } else {
        updateButton.disabled = true;   
    }
}

// Formdaki inputlara event listener ekle
let formChanged = false;
document.querySelectorAll("#Name, #ExamType, #Description").forEach(element => {
    element.addEventListener('input', () => {
        formChanged = true;  
        enableUpdateButton(); 
    });
});

// Başta butonun doğru durumunu kontrol et
enableUpdateButton();

// Eğer localization verileri varsa bunları al
const localizationData = document.getElementById('localization-data');
const unsavedChangesTitle = localizationData.getAttribute('data-unsaved-changes-title');
const unsavedChangesText = localizationData.getAttribute('data-unsaved-changes-text');
const okButtonText = localizationData.getAttribute('data-ok-button');
const cancelButtonText = localizationData.getAttribute('data-cancel-button');

async function deleteRule(index) {
    examRuleSubjects.splice(index, 1);
    examRuleSubjectsTable.splice(index, 1);
    ruleDeleted = true;
    refreshModal();
    enableUpdateButton();
}

//Function for populating select lists
async function populateSelectList(selectListId, data) {
    let selectListOptions = data.map((item, index) => `<option value="${item.value}">${item.text}</option>`);
    let selectList = `<option value="" disabled="" selected=""> ${localizedTexts.chooseText || "Choose"}</option>`.concat(selectListOptions);
    document.getElementById(selectListId).innerHTML = selectList;
}

async function refreshModal() {
    updateTable();
    clearExamRuleInputs()
    document.getElementById("examRuleSubjects").value = JSON.stringify(examRuleSubjects);
}

//Update table
async function updateTable() {
    document.getElementById("tableContent").innerHTML = examRuleSubjectsTable.map((item, index) =>
        `<tr>
            <td class="col-sm-2">${item.SubjectName}</td>
            <td class="col-sm-2">${item.SubtopicName}</td>
            <td class="col-sm-2">${item.ExamType}</td>
            <td class="col-sm-2">${item.QuestionType}</td>
            <td class="col-sm-2">${item.QuestionDifficulty}</td>
            <td class="col-sm-2">${item.QuestionCount}</td>
            <td class="col-sm-2"><button class="btn btn-danger btn-sm" type="button" onclick="deleteRule(${index})">X</button></td>
        </tr>`).join('');
}


async function clearExamRuleInputs() {
    document.getElementById("SubjectId").value = "";
    document.getElementById("SubtopicId").value = "";
    document.getElementById("QuestionType").value = "";
    document.getElementById("QuestionDifficultyId").value = "";
    document.getElementById("QuestionCount").value = "";
}

//Ajax functions for getting selectList
async function getExamRuleSubjects(examRuleId) {
    return $.ajax({
        url: '/Admin/ExamRule/GetExamRuleSubjectsByExamRuleId',
        data: { examRuleId: examRuleId },
    });
}

//Ajax functions for getting selectList
async function getSubjects(selectedProductId) {
    return $.ajax({
        url: '/Admin/ExamRule/GetSubjectsByProductId',
        data: { productId: selectedProductId },
    });
}

async function getSubtopics(selectedSubjectId) {
    return $.ajax({
        url: '/Admin/ExamRule/GetSubtopicsBySubjectId',
        data: { subjectId: selectedSubjectId },
    });
}

async function getQuestionTypes(selectedExamTypeId) {

    return $.ajax({
        url: '/Admin/ExamRule/GetQuestionTypes',
        data: { examTypeId: selectedExamTypeId }
    });
}

async function getQuestionDifficulties(selectedExamTypeId) {

    return $.ajax({
        url: '/Admin/ExamRule/GetQuestionDifficulties',
        data: { examTypeId: selectedExamTypeId }
    });
}

async function loadExamTypes() {
    examTypes = examTypes ? await getExamTypes() : examTypes;

    populateSelectList("ExamType", examTypes);
}

function getExamTypes() {
    return $.ajax({
        url: '/Admin/ExamRule/GetExamTypes'
    });
}

async function getSubjectNameBySubjectId(subjectId) {

    return $.ajax({
        url: '/Admin/ExamRule/GetSubjectNameBySubjectId',
        data: { subjectId: subjectId }
    });
}

function handleModalClose(event) {
    if (formChanged) {
        event.preventDefault();
        Swal.fire({
            title: unsavedChangesTitle,
            text: unsavedChangesText,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: okButtonText,
            cancelButtonText: cancelButtonText
        }).then((result) => {
            if (result.isConfirmed) {
                formChanged = false;
                window.history.back();
            }
        });
    } else {

        window.history.back();
    }
}

async function onProductChange() {
    if (!isEditing) {
        examRuleSubjects = [];
        examRuleSubjectsTable = [];
    }

    subjects = subjects ? await getSubjects($("#ProductId").val()) : subjects;
    populateSelectList("SubjectId", subjects);
    updateTable();
    document.getElementById("examRuleSubjects").value = JSON.stringify(examRuleSubjects);
}
$('#update-form').on('submit', function (e) {
    e.preventDefault();

    // Önceki doğrulama mesajlarını temizle (sadece submit sırasında kontrol edilecek alanlar için)
    $("#Name").next(".text-danger").text("");
    $("#examTypeValidation").text("");
    $("#Description").next(".text-danger").text("");
    $("#noRuleValidation").text("").attr("hidden", true);

    // Diğer alanların hata mesajlarını temizle (görünmesin diye)
    $("#productValidation").text("");
    $("#subjectValidation").text("");
    $("#subtopicValidation").text("");
    $("#questionTypeValidation").text("");
    $("#questionDifficultyValidation").text("");
    $("#questionCountValidation").text("");

    // Form alanlarının değerlerini al
    var name = $("#Name").val().trim(); // Boşlukları temizle
    var examType = $("#ExamType").val();
    var description = $("#Description").val().trim(); // Boşlukları temizle

    // Doğrulama başarılı mı kontrolü için bayrak
    var isValid = true;

    // Name alanını kontrol et
    if (!name) {
        isValid = false;
    }

    // ExamType alanını kontrol et
    if (!examType) {
        $("#examTypeValidation").text(localizedTexts.examTypeRequired);
        isValid = false;
    }

    // Description alanını kontrol et
    if (!description) {
        isValid = false;
    }

    // En az bir kural eklenmiş mi kontrol et
    if (examRuleSubjects.length === 0) {
        $("#noRuleValidation").text(localizedTexts.addAtLeastOneRule).removeAttr("hidden");
        isValid = false;
    } else {
        $("#noRuleValidation").attr("hidden", true);
    }

    // Eğer doğrulama başarısızsa, formu gönderme
    if (!isValid) {
        return;
    }

    $.ajax({
        url: '/Admin/ExamRule/Update',
        type: 'POST',
        data: $(this).serialize(),
        success: function (response) {
            console.log('Form başarıyla güncellendi.');
            window.location.href = '/Admin/ExamRule/Index';
        },
        error: function (xhr, status, error) {
            console.log('Bir hata oluştu: ' + error);
        }
    });
});

