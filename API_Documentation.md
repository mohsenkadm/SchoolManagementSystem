# 📘 SchoolMS API - توثيق كامل لجميع الـ Endpoints

## 📋 معلومات عامة

| الخاصية | القيمة |
|---------|--------|
| **اسم المشروع** | School Management System API |
| **إصدار .NET** | .NET 8 |
| **نوع المصادقة** | JWT Bearer Token |
| **Base URL** | `https://{domain}/api` |
| **Swagger** | متاح في بيئة التطوير على `/swagger` |
| **Rate Limiting** | `auth`: 10 طلب/دقيقة — `api`: 100 طلب/دقيقة |
| **CORS** | مُفعّل مع أصول محددة في `AllowedOrigins` |
| **SignalR Hubs** | `/hubs/chat` — `/hubs/notifications` — `/hubs/livestream-chat` |

---

## 🔐 المصادقة (Authentication)

> التوكن يحتوي على Claims: `UserType` (Teacher/Student/Parent/Staff) — `PersonId` — `Name` — `SchoolId`

### الهيدر المطلوب لجميع الطلبات المحمية:
```
Authorization: Bearer {token}
```

---

## 📑 فهرس الأقسام

1. [المصادقة وتسجيل الدخول (Auth)](#1-المصادقة-وتسجيل-الدخول-auth)
2. [تسجيل الطلاب (Student Registration)](#2-تسجيل-الطلاب-student-registration)
3. [المدرسة (School)](#3-المدرسة-school)
4. [السنوات الدراسية (Academic Years)](#4-السنوات-الدراسية-academic-years)
5. [الفروع (Branches)](#5-الفروع-branches)
6. [المراحل الدراسية (Grades)](#6-المراحل-الدراسية-grades)
7. [الشعب (Divisions)](#7-الشعب-divisions)
8. [المواد الدراسية (Subjects)](#8-المواد-الدراسية-subjects)
9. [الفصول الدراسية (ClassRooms)](#9-الفصول-الدراسية-classrooms)
10. [الحضور والانصراف (Attendance)](#10-الحضور-والانصراف-attendance)
11. [درجات الطلاب (Student Grades)](#11-درجات-الطلاب-student-grades)
12. [الواجبات المنزلية (Homework)](#12-الواجبات-المنزلية-homework)
13. [الاختبارات الإلكترونية (Quizzes)](#13-الاختبارات-الإلكترونية-quizzes)
14. [جداول الامتحانات (Exam Schedule)](#14-جداول-الامتحانات-exam-schedule)
15. [أنواع الامتحانات (Exam Types)](#15-أنواع-الامتحانات-exam-types)
16. [الجدول الأسبوعي (Weekly Schedule)](#16-الجدول-الأسبوعي-weekly-schedule)
17. [تعيينات المدرسين (Teacher Assignments)](#17-تعيينات-المدرسين-teacher-assignments)
18. [الإعلانات (Announcements)](#18-الإعلانات-announcements)
19. [الإشعارات (Notifications)](#19-الإشعارات-notifications)
20. [الأحداث المدرسية (Events)](#20-الأحداث-المدرسية-events)
21. [طلبات الإجازات (Leaves)](#21-طلبات-الإجازات-leaves)
22. [سلوك الطلاب (Behavior)](#22-سلوك-الطلاب-behavior)
23. [السجلات الصحية (Health Records)](#23-السجلات-الصحية-health-records)
24. [الرواتب (Salaries)](#24-الرواتب-salaries)
25. [أرباح المدرسين (Teacher Earnings)](#25-أرباح-المدرسين-teacher-earnings)
26. [المحادثات (Chat)](#26-المحادثات-chat)
27. [صور الكاروسيل (Carousel)](#27-صور-الكاروسيل-carousel)
28. [اشتراكات الطلاب (Student Subscriptions)](#28-اشتراكات-الطلاب-student-subscriptions)
29. [أكواد الخصم (Promo Codes)](#29-أكواد-الخصم-promo-codes)
30. [خطط الاشتراك (Subscription Plans)](#30-خطط-الاشتراك-subscription-plans)
31. [الكورسات (Courses)](#31-الكورسات-courses)
32. [فيديوهات الكورسات (Course Videos)](#32-فيديوهات-الكورسات-course-videos)
33. [البث المباشر (Live Streams)](#33-البث-المباشر-live-streams)
34. [المكتبة (Library)](#34-المكتبة-library)
35. [الأقساط والمدفوعات (Installments)](#35-الأقساط-والمدفوعات-installments)
36. [الموارد البشرية - الموظفين (HR Employees)](#36-الموارد-البشرية---الموظفين-hr-employees)
37. [الموارد البشرية - الأقسام (HR Departments)](#37-الموارد-البشرية---الأقسام-hr-departments)
38. [الموارد البشرية - المسميات الوظيفية (HR Job Titles)](#38-الموارد-البشرية---المسميات-الوظيفية-hr-job-titles)
39. [الموارد البشرية - الدرجات الوظيفية (HR Job Grades)](#39-الموارد-البشرية---الدرجات-الوظيفية-hr-job-grades)
40. [الموارد البشرية - الرواتب (HR Salary)](#40-الموارد-البشرية---الرواتب-hr-salary)
41. [الموارد البشرية - الحضور (HR Attendance)](#41-الموارد-البشرية---الحضور-hr-attendance)
42. [الموارد البشرية - العقود (HR Contracts)](#42-الموارد-البشرية---العقود-hr-contracts)
43. [الموارد البشرية - الإجازات (HR Leave)](#43-الموارد-البشرية---الإجازات-hr-leave)
44. [الموارد البشرية - العمل الإضافي (HR Overtime)](#44-الموارد-البشرية---العمل-الإضافي-hr-overtime)
45. [الموارد البشرية - الأداء (HR Performance)](#45-الموارد-البشرية---الأداء-hr-performance)
46. [الموارد البشرية - الترقيات (HR Promotions)](#46-الموارد-البشرية---الترقيات-hr-promotions)
47. [الموارد البشرية - ورديات العمل (HR Work Shifts)](#47-الموارد-البشرية---ورديات-العمل-hr-work-shifts)
48. [الموارد البشرية - التدريب (HR Training)](#48-الموارد-البشرية---التدريب-hr-training)
49. [الموارد البشرية - الإجراءات التأديبية (HR Disciplinary)](#49-الموارد-البشرية---الإجراءات-التأديبية-hr-disciplinary)
50. [SignalR Hubs (الاتصال في الوقت الحقيقي)](#50-signalr-hubs)

---

## 1. المصادقة وتسجيل الدخول (Auth)

**Base Route:** `api/Auth`  
**Rate Limiting:** `auth` (10 طلب/دقيقة)

| # | Method | Endpoint | الوصف | Auth | Request Body | Response |
|---|--------|----------|-------|------|-------------|----------|
| 1 | `POST` | `/api/Auth/teacher-login` | تسجيل دخول المعلم | ❌ | `PortalLoginDto` | `PortalLoginResultDto` |
| 2 | `POST` | `/api/Auth/student-login` | تسجيل دخول الطالب (مع فحص الجهاز) | ❌ | `PortalLoginDto` | `PortalLoginResultDto` |
| 3 | `POST` | `/api/Auth/parent-login` | تسجيل دخول ولي الأمر | ❌ | `PortalLoginDto` | `PortalLoginResultDto` |
| 4 | `POST` | `/api/Auth/staff-login` | تسجيل دخول الموظف | ❌ | `PortalLoginDto` | `PortalLoginResultDto` |
| 5 | `GET` | `/api/Auth/profile` | جلب بيانات المستخدم الحالي | ✅ | — | `UserProfileDto` |
| 6 | `POST` | `/api/Auth/student-logout-device` | تسجيل خروج الطالب من الجهاز | ✅ Student | — | `{ success, message }` |

### DTOs:

**PortalLoginDto:**
```json
{
  "username": "string",
  "password": "string",
  "deviceId": "string (optional - للطلاب فقط)"
}
```

**PortalLoginResultDto:**
```json
{
  "succeeded": true,
  "token": "JWT token string",
  "error": "string (optional)",
  "errorMessage": "string (optional)"
}
```

**UserProfileDto:**
```json
{
  "id": 0,
  "fullName": "string",
  "userType": "Teacher|Student|Parent|Staff",
  "phone": "string",
  "email": "string",
  "profileImage": "string",
  "username": "string",
  "schoolId": 0,
  "branchId": 0,
  "branchName": "string",
  "specialization": "string",
  "dateOfBirth": "date",
  "gender": "string",
  "address": "string",
  "position": "string"
}
```

---

## 2. تسجيل الطلاب (Student Registration)

**Base Route:** `api/StudentRegistration`  
**Rate Limiting:** `auth` (10 طلب/دقيقة)

| # | Method | Endpoint | الوصف | Auth | Request Body | Response |
|---|--------|----------|-------|------|-------------|----------|
| 1 | `POST` | `/api/StudentRegistration/send-otp` | إرسال كود OTP لرقم الهاتف عبر WhatsApp | ❌ | `SendOtpRequestDto` | `SendOtpResponseDto` |
| 2 | `POST` | `/api/StudentRegistration/verify-otp` | التحقق من كود OTP | ❌ | `VerifyOtpRequestDto` | `VerifyOtpResponseDto` |
| 3 | `POST` | `/api/StudentRegistration/register` | إكمال تسجيل الطالب بعد التحقق | ❌ | `RegisterStudentWithOtpDto` | `StudentDto` |
| 4 | `POST` | `/api/StudentRegistration/forgot-password/send-otp` | إرسال OTP لاستعادة كلمة المرور | ❌ | `SendOtpRequestDto` | `SendOtpResponseDto` |
| 5 | `POST` | `/api/StudentRegistration/forgot-password/verify-otp` | التحقق من OTP لاستعادة كلمة المرور | ❌ | `VerifyOtpRequestDto` | `VerifyOtpResponseDto` |
| 6 | `POST` | `/api/StudentRegistration/forgot-password/reset` | إعادة تعيين كلمة المرور | ❌ | `ResetPasswordDto` | `{ message }` |

### DTOs:

**SendOtpRequestDto:**
```json
{ "phone": "string" }
```

**VerifyOtpRequestDto:**
```json
{ "phone": "string", "code": "string" }
```

**RegisterStudentWithOtpDto:**
```json
{
  "verificationToken": "string",
  "fullName": "string",
  "fullNameAr": "string",
  "dateOfBirth": "date",
  "gender": "string",
  "nationalId": "string",
  "address": "string",
  "phone": "string",
  "parentPhone": "string",
  "parentName": "string",
  "email": "string",
  "username": "string",
  "password": "string",
  "branchId": 0,
  "classRoomId": 0,
  "academicYearId": 0,
  "notes": "string"
}
```

**ResetPasswordDto:**
```json
{
  "phone": "string",
  "verificationToken": "string",
  "newPassword": "string"
}
```

---

## 3. المدرسة (School)

**Base Route:** `api/{schoolId}/School`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/School` | جلب بيانات المدرسة وفروعها | ✅ |

**Response:** `List<SchoolDto>`

---

## 4. السنوات الدراسية (Academic Years)

**Base Route:** `api/{schoolId}/academic-years`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/academic-years` | جلب جميع السنوات الدراسية | ✅ |
| 2 | `GET` | `/api/{schoolId}/academic-years/current` | جلب السنة الدراسية الحالية | ✅ |

**Response:** `List<AcademicYearDto>` / `AcademicYearDto`

---

## 5. الفروع (Branches)

**Base Route:** `api/{schoolId}/branches`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/branches` | جلب جميع فروع المدرسة | ✅ |

**Response:** `List<BranchDto>`

---

## 6. المراحل الدراسية (Grades)

**Base Route:** `api/{schoolId}/grades`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/grades` | جلب المراحل الدراسية | ✅ | `branchId?` |

**Response:** `List<GradeDto>`

---

## 7. الشعب (Divisions)

**Base Route:** `api/{schoolId}/divisions`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/divisions` | جلب جميع الشعب | ✅ |

**Response:** `List<DivisionDto>`

---

## 8. المواد الدراسية (Subjects)

**Base Route:** `api/{schoolId}/subjects`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/subjects` | جلب جميع المواد الدراسية | ✅ |

**Response:** `List<SubjectDto>`

---

## 9. الفصول الدراسية (ClassRooms)

**Base Route:** `api/{schoolId}/classrooms`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/classrooms` | جلب الفصول الدراسية | ✅ | `branchId?` |

**Response:** `List<ClassRoomDto>`

---

## 10. الحضور والانصراف (Attendance)

**Base Route:** `api/{schoolId}/attendance`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/attendance/teacher` | حضور المعلم (من التوكن) | ✅ Teacher | `dateFrom?`, `dateTo?`, `type?`, `academicYearId?` |
| 2 | `GET` | `/api/{schoolId}/attendance/staff` | حضور الموظف (من التوكن) | ✅ Staff | `dateFrom?`, `dateTo?`, `type?`, `academicYearId?` |
| 3 | `GET` | `/api/{schoolId}/attendance/student` | حضور الطالب (من التوكن) | ✅ Student | `dateFrom?`, `dateTo?`, `type?`, `academicYearId?` |
| 4 | `GET` | `/api/{schoolId}/attendance/parent/children` | حضور أبناء ولي الأمر | ✅ Parent | `dateFrom?`, `dateTo?`, `type?`, `academicYearId?` |

**Response:** `List<AttendanceDto>`

**AttendanceType Enum:** تحدد نوع الحضور (حاضر، غائب، متأخر، إلخ)

---

## 11. درجات الطلاب (Student Grades)

**Base Route:** `api/{schoolId}/student-grades`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/student-grades` | جميع درجات المدرسة | ✅ | `subjectId?`, `examTypeId?`, `academicYearId?` |
| 2 | `GET` | `/api/{schoolId}/student-grades/student` | درجات الطالب | ✅ Student | `subjectId?`, `examTypeId?`, `academicYearId?` |
| 3 | `GET` | `/api/{schoolId}/student-grades/teacher` | درجات فصول المعلم | ✅ Teacher | `subjectId?`, `examTypeId?`, `academicYearId?` |
| 4 | `GET` | `/api/{schoolId}/student-grades/parent/children` | درجات أبناء ولي الأمر | ✅ Parent | `subjectId?`, `examTypeId?`, `academicYearId?` |

**Response:** `List<StudentGradeDto>`

---

## 12. الواجبات المنزلية (Homework)

**Base Route:** `api/{schoolId}/homework`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/homework` | جميع الواجبات | ✅ | `branchId?`, `classRoomId?`, `subjectId?`, `teacherId?`, `academicYearId?` |
| 2 | `GET` | `/api/{schoolId}/homework/student` | واجبات الطالب | ✅ Student | `subjectId?`, `academicYearId?` |
| 3 | `GET` | `/api/{schoolId}/homework/teacher` | واجبات المعلم | ✅ Teacher | `classRoomId?`, `subjectId?`, `academicYearId?` |
| 4 | `GET` | `/api/{schoolId}/homework/parent/children` | واجبات أبناء ولي الأمر | ✅ Parent | `subjectId?`, `academicYearId?` |

**Response:** `List<HomeworkDto>`

---

## 13. الاختبارات الإلكترونية (Quizzes)

**Base Route:** `api/{schoolId}/quizzes`

### مجموعات الاختبارات:

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/quizzes` | جميع مجموعات الاختبارات | ✅ | `branchId?`, `teacherId?`, `subjectId?`, `academicYearId?`, `classRoomId?` |
| 2 | `GET` | `/api/{schoolId}/quizzes/{id}` | مجموعة اختبار بالمعرف | ✅ | — |
| 3 | `POST` | `/api/{schoolId}/quizzes` | إنشاء مجموعة اختبار | ✅ | Body: `QuizGroupDto` |
| 4 | `DELETE` | `/api/{schoolId}/quizzes/{id}` | حذف مجموعة اختبار | ✅ | — |

### الأسئلة:

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 5 | `GET` | `/api/{schoolId}/quizzes/{groupId}/questions` | أسئلة اختبار | ✅ |
| 6 | `POST` | `/api/{schoolId}/quizzes/questions` | إضافة سؤال | ✅ |
| 7 | `DELETE` | `/api/{schoolId}/quizzes/questions/{id}` | حذف سؤال | ✅ |

### اختبارات الطالب:

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 8 | `GET` | `/api/{schoolId}/quizzes/student` | اختبارات الطالب | ✅ Student | `subjectId?`, `teacherId?` |
| 9 | `POST` | `/api/{schoolId}/quizzes/{groupId}/submit` | تسليم إجابات الطالب | ✅ Student | Body: `List<SubmitQuizAnswerDto>` |
| 10 | `GET` | `/api/{schoolId}/quizzes/{groupId}/my-answers` | إجابات الطالب | ✅ Student | — |

### اختبارات المعلم:

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 11 | `GET` | `/api/{schoolId}/quizzes/teacher` | اختبارات المعلم | ✅ Teacher | `subjectId?`, `classRoomId?`, `academicYearId?` |
| 12 | `GET` | `/api/{schoolId}/quizzes/teacher/{groupId}/questions` | أسئلة اختبار المعلم | ✅ Teacher | — |
| 13 | `GET` | `/api/{schoolId}/quizzes/teacher/{groupId}/answers` | جميع إجابات الطلاب | ✅ Teacher | `classRoomId?` |
| 14 | `GET` | `/api/{schoolId}/quizzes/{groupId}/answers/{studentId}` | إجابات طالب معين | ✅ | — |

### اختبارات ولي الأمر:

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 15 | `GET` | `/api/{schoolId}/quizzes/parent/children` | اختبارات الأبناء | ✅ Parent | `subjectId?` |
| 16 | `GET` | `/api/{schoolId}/quizzes/parent/{groupId}/answers/{studentId}` | إجابات ابن معين | ✅ Parent | — |

---

## 14. جداول الامتحانات (Exam Schedule)

**Base Route:** `api/{schoolId}/exam-schedule`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/exam-schedule` | جميع جداول الامتحانات | ✅ | `branchId?`, `examTypeId?`, `classRoomId?`, `subjectId?`, `teacherId?`, `academicYearId?` |
| 2 | `GET` | `/api/{schoolId}/exam-schedule/student` | جدول امتحانات الطالب | ✅ Student | — |
| 3 | `GET` | `/api/{schoolId}/exam-schedule/teacher` | جدول امتحانات المعلم | ✅ Teacher | `examTypeId?`, `subjectId?`, `classRoomId?`, `academicYearId?` |
| 4 | `GET` | `/api/{schoolId}/exam-schedule/parent/children` | جدول امتحانات الأبناء | ✅ Parent | — |

**Response:** `List<ExamScheduleDto>`

---

## 15. أنواع الامتحانات (Exam Types)

**Base Route:** `api/{schoolId}/exam-types`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/exam-types` | جميع أنواع الامتحانات | ✅ |

**Response:** `List<ExamTypeDto>`

---

## 16. الجدول الأسبوعي (Weekly Schedule)

**Base Route:** `api/{schoolId}/weekly-schedule`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/weekly-schedule` | الجدول الأسبوعي الكامل | ✅ | `subjectId?`, `teacherId?`, `dayOfWeek?`, `classRoomId?` |
| 2 | `GET` | `/api/{schoolId}/weekly-schedule/student` | جدول الطالب | ✅ Student | `subjectId?`, `dayOfWeek?` |
| 3 | `GET` | `/api/{schoolId}/weekly-schedule/teacher` | جدول المعلم | ✅ Teacher | `subjectId?`, `dayOfWeek?` |
| 4 | `GET` | `/api/{schoolId}/weekly-schedule/parent/children` | جدول الأبناء | ✅ Parent | `subjectId?`, `dayOfWeek?` |

**Response:** `List<WeeklyScheduleDto>`  
**dayOfWeek:** `0=Sunday, 1=Monday, ... 6=Saturday`

---

## 17. تعيينات المدرسين (Teacher Assignments)

**Base Route:** `api/{schoolId}/teacher-assignments`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/teacher-assignments` | جميع التعيينات | ✅ |
| 2 | `GET` | `/api/{schoolId}/teacher-assignments/student` | تعيينات فصل الطالب | ✅ Student |
| 3 | `GET` | `/api/{schoolId}/teacher-assignments/teacher` | تعيينات المعلم | ✅ Teacher |
| 4 | `GET` | `/api/{schoolId}/teacher-assignments/parent/children` | تعيينات فصول الأبناء | ✅ Parent |

**Response:** `List<TeacherAssignmentDto>`

---

## 18. الإعلانات (Announcements)

**Base Route:** `api/{schoolId}/announcements`

| # | Method | Endpoint | الوصف | Auth | Query/Body |
|---|--------|----------|-------|------|-----------|
| 1 | `GET` | `/api/{schoolId}/announcements` | جميع الإعلانات | ✅ | `branchId?` |
| 2 | `GET` | `/api/{schoolId}/announcements/{id}` | إعلان بالمعرف | ✅ | — |
| 3 | `POST` | `/api/{schoolId}/announcements` | إنشاء إعلان | ✅ | Body: `AnnouncementDto` |
| 4 | `PUT` | `/api/{schoolId}/announcements/{id}` | تعديل إعلان | ✅ | Body: `AnnouncementDto` |
| 5 | `DELETE` | `/api/{schoolId}/announcements/{id}` | حذف إعلان | ✅ | — |

**Response:** `AnnouncementDto`

---

## 19. الإشعارات (Notifications)

**Base Route:** `api/{schoolId}/notifications`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/notifications` | جميع إشعارات المدرسة | ✅ |
| 2 | `GET` | `/api/{schoolId}/notifications/student` | إشعارات الطالب | ✅ Student |
| 3 | `GET` | `/api/{schoolId}/notifications/teacher` | إشعارات المعلم | ✅ Teacher |
| 4 | `GET` | `/api/{schoolId}/notifications/parent` | إشعارات ولي الأمر | ✅ Parent |
| 5 | `GET` | `/api/{schoolId}/notifications/staff` | إشعارات الموظف | ✅ Staff |
| 6 | `POST` | `/api/{schoolId}/notifications/{id}/send` | تحديد إشعار كمرسل | ✅ |

**Response:** `List<NotificationDto>`

---

## 20. الأحداث المدرسية (Events)

**Base Route:** `api/{schoolId}/events`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/events` | جميع أحداث المدرسة | ✅ | `branchId?` |

**Response:** `List<SchoolEventDto>`

---

## 21. طلبات الإجازات (Leaves)

**Base Route:** `api/{schoolId}/leaves`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/leaves` | جميع طلبات الإجازات | ✅ | `status?`, `personType?` |
| 2 | `GET` | `/api/{schoolId}/leaves/student` | إجازات الطالب | ✅ Student | `status?` |
| 3 | `GET` | `/api/{schoolId}/leaves/parent/children` | إجازات الأبناء | ✅ Parent | `status?` |
| 4 | `POST` | `/api/{schoolId}/leaves` | إنشاء طلب إجازة | ✅ | Body: `LeaveRequestDto` |

**Response:** `List<LeaveRequestDto>`  
**LeaveStatus:** `Pending, Approved, Rejected`  
**PersonType:** `Teacher, Student, Staff`

---

## 22. سلوك الطلاب (Behavior)

**Base Route:** `api/{schoolId}/behavior`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/behavior/student/{studentId}` | سجلات سلوك طالب | ✅ | `academicYearId?` |
| 2 | `GET` | `/api/{schoolId}/behavior/parent/children` | سلوك أبناء ولي الأمر | ✅ Parent | `academicYearId?` |

**Response:** `List<StudentBehaviorDto>`

---

## 23. السجلات الصحية (Health Records)

**Base Route:** `api/{schoolId}/health-records`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/health-records` | جميع السجلات الصحية | ✅ | `studentId?`, `academicYearId?` |
| 2 | `GET` | `/api/{schoolId}/health-records/parent/children` | سجلات الأبناء الصحية | ✅ Parent | `academicYearId?` |

**Response:** `List<HealthRecordDto>`

---

## 24. الرواتب (Salaries)

**Base Route:** `api/{schoolId}/salaries`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/salaries/my-salary` | راتب المستخدم الحالي | ✅ Teacher/Staff |

**Response:** `SalarySetupDto`

---

## 25. أرباح المدرسين (Teacher Earnings)

**Base Route:** `api/{schoolId}/teacher-earnings`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/teacher-earnings/my-earnings` | أرباح المدرس الحالي | ✅ Teacher |
| 2 | `GET` | `/api/{schoolId}/teacher-earnings/my-summary` | ملخص أرباح المدرس | ✅ Teacher |
| 3 | `GET` | `/api/{schoolId}/teacher-earnings` | جميع الأرباح (للمشرف) | ✅ |
| 4 | `GET` | `/api/{schoolId}/teacher-earnings/teacher/{teacherId}` | أرباح مدرس معين | ✅ |
| 5 | `GET` | `/api/{schoolId}/teacher-earnings/summary` | ملخص أرباح جميع المدرسين | ✅ |

**Response:** `List<TeacherEarningDto>` / `TeacherEarningSummaryDto`

---

## 26. المحادثات (Chat)

**Base Route:** `api/{schoolId}/chat`

| # | Method | Endpoint | الوصف | Auth | Query/Body |
|---|--------|----------|-------|------|-----------|
| 1 | `GET` | `/api/{schoolId}/chat/rooms` | غرف المحادثة | ✅ | `branchId?` |
| 2 | `GET` | `/api/{schoolId}/chat/rooms/student/{studentId}` | غرف الطالب | ✅ | — |
| 3 | `GET` | `/api/{schoolId}/chat/rooms/teacher/{teacherId}` | غرف المعلم | ✅ | — |
| 4 | `POST` | `/api/{schoolId}/chat/rooms` | إنشاء غرفة (معلم/أدمن) | ✅ | Body: `ChatRoomDto` |
| 5 | `GET` | `/api/{schoolId}/chat/rooms/{roomId}/messages` | رسائل الغرفة | ✅ | — |
| 6 | `POST` | `/api/{schoolId}/chat/messages` | إرسال رسالة نصية | ✅ | Body: `ChatMessageDto` |
| 7 | `POST` | `/api/{schoolId}/chat/messages/upload` | إرسال رسالة مع ملف | ✅ | `FormData` (max 20MB) |

### ChatMessageDto:
```json
{
  "chatRoomId": 0,
  "senderId": 0,
  "senderType": "Teacher|Student",
  "senderName": "string",
  "message": "string",
  "fileUrl": "string",
  "fileType": "string"
}
```

**الملفات المسموحة:** `image/jpeg`, `image/png`, `image/gif`, `application/pdf`

---

## 27. صور الكاروسيل (Carousel)

**Base Route:** `api/{schoolId}/carousel`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/carousel` | صور الكاروسيل | ✅ | `branchId?` |

**Response:** `List<CarouselImageDto>`

---

## 28. اشتراكات الطلاب (Student Subscriptions)

**Base Route:** `api/{schoolId}/student-subscriptions`

| # | Method | Endpoint | الوصف | Auth | Body |
|---|--------|----------|-------|------|------|
| 1 | `GET` | `/api/{schoolId}/student-subscriptions/student` | اشتراكات الطالب | ✅ Student | — |
| 2 | `POST` | `/api/{schoolId}/student-subscriptions/subscribe` | اشتراك في خطة جديدة | ✅ | `SubscribeRequestDto` |

### SubscribeRequestDto:
```json
{
  "studentId": 0,
  "onlineSubscriptionPlanId": 0,
  "promoCode": "string (optional)",
  "schoolId": 0
}
```

**Response:** `StudentSubscriptionDto`

---

## 29. أكواد الخصم (Promo Codes)

**Base Route:** `api/{schoolId}/promo-codes`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/promo-codes` | جميع أكواد الخصم | ✅ |
| 2 | `GET` | `/api/{schoolId}/promo-codes/student` | أكواد الخصم النشطة للطالب | ✅ Student |
| 3 | `GET` | `/api/{schoolId}/promo-codes/code/{code}` | كود خصم بالنص | ✅ |

**Response:** `List<PromoCodeDto>` / `PromoCodeDto`

---

## 30. خطط الاشتراك (Subscription Plans)

**Base Route:** `api/{schoolId}/subscription-plans`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/subscription-plans` | جميع خطط الاشتراك | ✅ | `search?` |
| 2 | `GET` | `/api/{schoolId}/subscription-plans/student` | خطط الاشتراك للطلاب | ✅ Student | `search?` |

**Response:** `List<OnlineSubscriptionPlanDto>`

---

## 31. الكورسات (Courses)

**Base Route:** `api/{schoolId}/courses`

| # | Method | Endpoint | الوصف | Auth | Query/Body |
|---|--------|----------|-------|------|-----------|
| 1 | `GET` | `/api/{schoolId}/courses` | جميع الكورسات (مفلترة حسب نوع المستخدم) | ✅ | `subjectId?`, `teacherId?` |
| 2 | `GET` | `/api/{schoolId}/courses/top` | أفضل الكورسات | ✅ | `count?` (default: 10) |
| 3 | `GET` | `/api/{schoolId}/courses/{id}` | كورس بالمعرف | ✅ | — |
| 4 | `POST` | `/api/{schoolId}/courses` | إنشاء كورس | ✅ | Body: `CreateCourseDto` |
| 5 | `PUT` | `/api/{schoolId}/courses` | تعديل كورس | ✅ | Body: `CourseDto` |
| 6 | `DELETE` | `/api/{schoolId}/courses/{id}` | حذف كورس | ✅ | — |

> **ملاحظة:** الطالب يرى فقط الكورسات المرتبطة بالمواد المشترك بها. المعلم يرى كورساته فقط.

---

## 32. فيديوهات الكورسات (Course Videos)

**Base Route:** `api/{schoolId}/course-videos`

### الفيديوهات الأساسية:

| # | Method | Endpoint | الوصف | Auth | Body/Params |
|---|--------|----------|-------|------|------------|
| 1 | `GET` | `/api/{schoolId}/course-videos/course/{courseId}` | فيديوهات كورس | ✅ | — |
| 2 | `GET` | `/api/{schoolId}/course-videos/course/{courseId}/free` | الفيديوهات المجانية لكورس معين | ❌ (AllowAnonymous) | — |
| 3 | `GET` | `/api/{schoolId}/course-videos/{id}` | فيديو بالمعرف | ✅ | — |
| 4 | `GET` | `/api/{schoolId}/course-videos/storage-quota` | حصة التخزين | ✅ | — |
| 5 | `POST` | `/api/{schoolId}/course-videos` | إنشاء فيديو | ✅ Teacher/Admin | Body: `CreateCourseVideoDto` |
| 6 | `PUT` | `/api/{schoolId}/course-videos` | تعديل فيديو | ✅ Teacher/Admin | Body: `CourseVideoDto` |
| 7 | `POST` | `/api/{schoolId}/course-videos/{courseVideoId}/upload` | رفع ملف فيديو (max 500MB) | ✅ Teacher/Admin | `FormFile` |
| 8 | `DELETE` | `/api/{schoolId}/course-videos/{id}` | حذف فيديو | ✅ Teacher/Admin | — |

### التفاعلات:

| # | Method | Endpoint | الوصف | Auth | Body |
|---|--------|----------|-------|------|------|
| 9 | `POST` | `/api/{schoolId}/course-videos/{id}/seen` | تحديد كمشاهد | ✅ | `int studentId` |
| 10 | `POST` | `/api/{schoolId}/course-videos/{id}/like` | إعجاب/إلغاء إعجاب | ✅ | `int studentId` |
| 11 | `POST` | `/api/{schoolId}/course-videos/{id}/rate` | تقييم (1-5) | ✅ | `VideoRateRequestDto` |

### التعليقات:

| # | Method | Endpoint | الوصف | Auth | Body |
|---|--------|----------|-------|------|------|
| 12 | `GET` | `/api/{schoolId}/course-videos/{courseVideoId}/comments` | تعليقات الفيديو | ✅ | — |
| 13 | `POST` | `/api/{schoolId}/course-videos/{courseVideoId}/comments` | إضافة تعليق | ✅ Student | `CreateVideoCommentDto` |

### اختبارات الفيديو (Video Quiz):

| # | Method | Endpoint | الوصف | Auth | Body |
|---|--------|----------|-------|------|------|
| 14 | `GET` | `/api/{schoolId}/course-videos/{courseVideoId}/quiz` | أسئلة اختبار الفيديو | ✅ | — |
| 15 | `POST` | `/api/{schoolId}/course-videos/{courseVideoId}/quiz` | إنشاء سؤال | ✅ Teacher/Admin | `CreateVideoQuizQuestionDto` |
| 16 | `PUT` | `/api/{schoolId}/course-videos/{courseVideoId}/quiz/{questionId}` | تعديل سؤال | ✅ Teacher/Admin | `VideoQuizQuestionDto` |
| 17 | `DELETE` | `/api/{schoolId}/course-videos/{courseVideoId}/quiz/{questionId}` | حذف سؤال | ✅ Teacher/Admin | — |
| 18 | `POST` | `/api/{schoolId}/course-videos/{courseVideoId}/quiz/submit` | تسليم إجابة | ✅ Student | `SubmitVideoQuizAnswerDto` |
| 19 | `GET` | `/api/{schoolId}/course-videos/{courseVideoId}/quiz/answers` | جميع الإجابات | ✅ Teacher/Admin | — |
| 20 | `GET` | `/api/{schoolId}/course-videos/{courseVideoId}/quiz/my-answers` | إجاباتي | ✅ Student | — |

### ملاحظات الفيديو (Video Notes):

| # | Method | Endpoint | الوصف | Auth | Body |
|---|--------|----------|-------|------|------|
| 21 | `GET` | `/api/{schoolId}/course-videos/{courseVideoId}/notes` | ملاحظاتي على الفيديو | ✅ Student | — |
| 22 | `POST` | `/api/{schoolId}/course-videos/{courseVideoId}/notes` | حفظ ملاحظة | ✅ Student | `SaveVideoNoteDto` |

### فحص الجهاز:

| # | Method | Endpoint | الوصف | Auth | Body |
|---|--------|----------|-------|------|------|
| 23 | `POST` | `/api/{schoolId}/course-videos/check-device` | فحص صلاحية الجهاز | ✅ Student | `string deviceId` |
| 24 | `POST` | `/api/{schoolId}/course-videos/logout-device` | تسجيل خروج من الجهاز | ✅ Student | — |

---

## 33. البث المباشر (Live Streams)

**Base Route:** `api/{schoolId}/live-streams`

| # | Method | Endpoint | الوصف | Auth | Query/Body |
|---|--------|----------|-------|------|-----------|
| 1 | `GET` | `/api/{schoolId}/live-streams` | جميع البثوث (مفلترة حسب المستخدم) | ✅ | `subjectId?`, `courseId?`, `teacherId?` |
| 2 | `GET` | `/api/{schoolId}/live-streams/course/{courseId}` | بثوث كورس معين | ✅ | — |
| 3 | `GET` | `/api/{schoolId}/live-streams/{id}` | بث بالمعرف | ✅ | — |
| 4 | `POST` | `/api/{schoolId}/live-streams` | إنشاء بث | ✅ Teacher/Admin | Body: `CreateLiveStreamDto` |
| 5 | `POST` | `/api/{schoolId}/live-streams/{id}/status` | تحديث حالة البث | ✅ Teacher/Admin | Body: `LiveStreamStatus` |
| 6 | `DELETE` | `/api/{schoolId}/live-streams/{id}` | حذف بث | ✅ Teacher/Admin | — |
| 7 | `POST` | `/api/{schoolId}/live-streams/{id}/seen` | تحديد كمشاهد | ✅ | Body: `int studentId` |
| 8 | `GET` | `/api/{schoolId}/live-streams/{liveStreamId}/comments` | تعليقات البث | ✅ | — |
| 9 | `POST` | `/api/{schoolId}/live-streams/{liveStreamId}/comments` | إضافة تعليق (مع SignalR) | ✅ Student/Teacher | Body: `CreateLiveStreamCommentDto` |

**LiveStreamStatus:** `Scheduled, Live, Ended`

---

## 34. المكتبة (Library)

**Base Route:** `api/{schoolId}/library`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/library/books` | جميع كتب المكتبة | ✅ | `branchId?`, `search?`, `category?` |
| 2 | `GET` | `/api/{schoolId}/library/books/{id}` | كتاب بالمعرف | ✅ | — |

**Response:** `List<LibraryBookDto>` / `LibraryBookDto`

---

## 35. الأقساط والمدفوعات (Installments)

**Base Route:** `api/{schoolId}/installments`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/installments` | جميع الأقساط | ✅ | `branchId?`, `academicYearId?`, `studentId?` |
| 2 | `GET` | `/api/{schoolId}/installments/student` | أقساط الطالب | ✅ Student | `academicYearId?` |
| 3 | `GET` | `/api/{schoolId}/installments/parent/children` | أقساط الأبناء | ✅ Parent | `academicYearId?` |

**Response:** `List<FeeInstallmentDto>`

---

## 36. الموارد البشرية - الموظفين (HR Employees)

**Base Route:** `api/{schoolId}/hr/employees`

| # | Method | Endpoint | الوصف | Auth | Body |
|---|--------|----------|-------|------|------|
| 1 | `GET` | `/api/{schoolId}/hr/employees` | جميع الموظفين | ✅ | — |
| 2 | `GET` | `/api/{schoolId}/hr/employees/{id}` | موظف بالمعرف | ✅ | — |
| 3 | `POST` | `/api/{schoolId}/hr/employees` | إضافة موظف | ✅ | `HrEmployeeDto` |
| 4 | `PUT` | `/api/{schoolId}/hr/employees` | تعديل موظف | ✅ | `HrEmployeeDto` |
| 5 | `DELETE` | `/api/{schoolId}/hr/employees/{id}` | حذف موظف | ✅ | — |
| 6 | `GET` | `/api/{schoolId}/hr/employees/department/{departmentId}` | موظفين قسم معين | ✅ | — |
| 7 | `GET` | `/api/{schoolId}/hr/employees/branch/{branchId}` | موظفين فرع معين | ✅ | — |
| 8 | `GET` | `/api/{schoolId}/hr/employees/generate-number` | توليد رقم موظف | ✅ | — |

---

## 37. الموارد البشرية - الأقسام (HR Departments)

**Base Route:** `api/{schoolId}/hr/departments`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/hr/departments` | جميع الأقسام | ✅ |

**Response:** `List<HrDepartmentDto>`

---

## 38. الموارد البشرية - المسميات الوظيفية (HR Job Titles)

**Base Route:** `api/{schoolId}/hr/job-titles`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/hr/job-titles` | جميع المسميات | ✅ |

**Response:** `List<HrJobTitleDto>`

---

## 39. الموارد البشرية - الدرجات الوظيفية (HR Job Grades)

**Base Route:** `api/{schoolId}/hr/job-grades`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/hr/job-grades` | جميع الدرجات الوظيفية | ✅ |
| 2 | `GET` | `/api/{schoolId}/hr/job-grades/{gradeId}/steps` | مراحل درجة وظيفية | ✅ |

**Response:** `List<HrJobGradeDto>` / `List<HrJobGradeStepDto>`

---

## 40. الموارد البشرية - الرواتب (HR Salary)

**Base Route:** `api/{schoolId}/hr/salary`

### إعداد الراتب:

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/hr/salary/employee/{employeeId}` | الراتب الحالي للموظف | ✅ | — |
| 2 | `GET` | `/api/{schoolId}/hr/salary/employee/{employeeId}/history` | سجل الرواتب | ✅ | — |

### أنواع البدلات والخصومات:

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 3 | `GET` | `/api/{schoolId}/hr/salary/allowance-types` | أنواع البدلات | ✅ |
| 4 | `GET` | `/api/{schoolId}/hr/salary/deduction-types` | أنواع الخصومات | ✅ |

### كشوفات الرواتب الشهرية (Payroll):

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 5 | `POST` | `/api/{schoolId}/hr/salary/payroll/generate` | إنشاء كشف رواتب | ✅ | `month`, `year`, `branchId` |
| 6 | `GET` | `/api/{schoolId}/hr/salary/payroll` | كشف رواتب معين | ✅ | `month`, `year`, `branchId` |
| 7 | `GET` | `/api/{schoolId}/hr/salary/payroll/list` | قائمة كشوفات | ✅ | `year?` |

### السلف والقروض والمكافآت والعقوبات:

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 8 | `GET` | `/api/{schoolId}/hr/salary/advances` | السلف | ✅ | `status?` |
| 9 | `GET` | `/api/{schoolId}/hr/salary/loans` | القروض | ✅ | `employeeId?` |
| 10 | `GET` | `/api/{schoolId}/hr/salary/bonuses` | المكافآت | ✅ | `month?`, `year?` |
| 11 | `GET` | `/api/{schoolId}/hr/salary/penalties` | العقوبات | ✅ | `month?`, `year?` |

---

## 41. الموارد البشرية - الحضور (HR Attendance)

**Base Route:** `api/{schoolId}/hr/attendance`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/hr/attendance/daily` | الحضور اليومي | ✅ | `date`, `departmentId?`, `branchId?` |
| 2 | `GET` | `/api/{schoolId}/hr/attendance/monthly/{employeeId}` | حضور شهري لموظف | ✅ | `month`, `year` |
| 3 | `POST` | `/api/{schoolId}/hr/attendance/process` | معالجة الحضور اليومي | ✅ | `date` |

---

## 42. الموارد البشرية - العقود (HR Contracts)

**Base Route:** `api/{schoolId}/hr/contracts`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/hr/contracts` | جميع العقود | ✅ |
| 2 | `GET` | `/api/{schoolId}/hr/contracts/employee/{employeeId}` | عقود موظف | ✅ |

**Response:** `List<HrEmployeeContractDto>`

---

## 43. الموارد البشرية - الإجازات (HR Leave)

**Base Route:** `api/{schoolId}/hr/leave`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/hr/leave/requests` | طلبات الإجازات | ✅ | `status?` |
| 2 | `POST` | `/api/{schoolId}/hr/leave/requests` | إنشاء طلب إجازة | ✅ | Body: `HrLeaveRequestDto` |
| 3 | `GET` | `/api/{schoolId}/hr/leave/types` | أنواع الإجازات | ✅ | — |
| 4 | `GET` | `/api/{schoolId}/hr/leave/balances/{employeeId}` | أرصدة إجازات موظف | ✅ | — |
| 5 | `GET` | `/api/{schoolId}/hr/leave/balances/all` | جميع الأرصدة | ✅ | `year` |
| 6 | `POST` | `/api/{schoolId}/hr/leave/balances/initialize` | تهيئة الأرصدة | ✅ | `employeeId`, `year` |
| 7 | `GET` | `/api/{schoolId}/hr/leave/holidays` | العطل الرسمية | ✅ | `year?` |

**HrLeaveStatus:** `Pending, Approved, Rejected`

---

## 44. الموارد البشرية - العمل الإضافي (HR Overtime)

**Base Route:** `api/{schoolId}/hr/overtime`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/hr/overtime` | طلبات العمل الإضافي | ✅ | `status?` |

**OvertimeStatus:** `Pending, Approved, Rejected`

---

## 45. الموارد البشرية - الأداء (HR Performance)

**Base Route:** `api/{schoolId}/hr/performance`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/hr/performance/cycles` | دورات تقييم الأداء | ✅ | — |
| 2 | `GET` | `/api/{schoolId}/hr/performance/criteria` | معايير التقييم | ✅ | — |
| 3 | `GET` | `/api/{schoolId}/hr/performance/reviews` | مراجعات الأداء | ✅ | `cycleId?`, `employeeId?` |
| 4 | `GET` | `/api/{schoolId}/hr/performance/kpis` | مؤشرات الأداء | ✅ | `employeeId?` |

---

## 46. الموارد البشرية - الترقيات (HR Promotions)

**Base Route:** `api/{schoolId}/hr/promotions`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/hr/promotions` | جميع الترقيات | ✅ | `status?` |
| 2 | `GET` | `/api/{schoolId}/hr/promotions/career-history/{employeeId}` | السجل الوظيفي | ✅ | — |

**HrPromotionStatus:** `Pending, Approved, Rejected`

---

## 47. الموارد البشرية - ورديات العمل (HR Work Shifts)

**Base Route:** `api/{schoolId}/hr/work-shifts`

| # | Method | Endpoint | الوصف | Auth |
|---|--------|----------|-------|------|
| 1 | `GET` | `/api/{schoolId}/hr/work-shifts` | جميع الورديات | ✅ |

**Response:** `List<HrWorkShiftDto>`

---

## 48. الموارد البشرية - التدريب (HR Training)

**Base Route:** `api/{schoolId}/hr/training`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/hr/training/programs` | برامج التدريب | ✅ | — |
| 2 | `GET` | `/api/{schoolId}/hr/training/records` | سجلات التدريب | ✅ | `programId?`, `employeeId?` |
| 3 | `GET` | `/api/{schoolId}/hr/training/certificates/{employeeId}` | شهادات الموظف | ✅ | — |

---

## 49. الموارد البشرية - الإجراءات التأديبية (HR Disciplinary)

**Base Route:** `api/{schoolId}/hr/disciplinary`

| # | Method | Endpoint | الوصف | Auth | Query Params |
|---|--------|----------|-------|------|-------------|
| 1 | `GET` | `/api/{schoolId}/hr/disciplinary` | الإجراءات التأديبية | ✅ | `employeeId?` |
| 2 | `GET` | `/api/{schoolId}/hr/disciplinary/violation-types` | أنواع المخالفات | ✅ | — |

---

## 50. SignalR Hubs

### 🔌 Hub 1: المحادثات `/hubs/chat`

**الاتصال:** يتطلب JWT Token عبر query string:
```
wss://{domain}/hubs/chat?access_token={JWT_TOKEN}
```

**Methods (Client → Server):**

| Method | Parameters | الوصف |
|--------|-----------|-------|
| `JoinRoom` | `roomId: string` | الانضمام لغرفة محادثة |
| `LeaveRoom` | `roomId: string` | مغادرة غرفة محادثة |
| `SendMessage` | `roomId, senderName, message, fileUrl?, fileType?` | إرسال رسالة |

**Events (Server → Client):**

| Event | Parameters | الوصف |
|-------|-----------|-------|
| `ReceiveMessage` | `senderName, message, fileUrl, fileType, sentAt` | استقبال رسالة |
| `UserJoined` | `userName` | مستخدم انضم |
| `UserLeft` | `userName` | مستخدم غادر |

---

### 🔔 Hub 2: الإشعارات `/hubs/notifications`

**الاتصال:**
```
wss://{domain}/hubs/notifications?access_token={JWT_TOKEN}
```

**Events (Server → Client):**

| Event | Parameters | الوصف |
|-------|-----------|-------|
| `ReceiveNotification` | `title, message, dateTime` | استقبال إشعار |

---

### 📺 Hub 3: البث المباشر `/hubs/livestream-chat`

**الاتصال:**
```
wss://{domain}/hubs/livestream-chat?access_token={JWT_TOKEN}
```

**Methods (Client → Server):**

| Method | Parameters | الوصف |
|--------|-----------|-------|
| `JoinLiveStream` | `liveStreamId: string` | الانضمام للبث |
| `LeaveLiveStream` | `liveStreamId: string` | مغادرة البث |
| `SendLiveComment` | `liveStreamId, studentName, senderType, comment` | إرسال تعليق |

**Events (Server → Client):**

| Event | Parameters | الوصف |
|-------|-----------|-------|
| `ReceiveLiveComment` | `senderName, senderType, comment, sentAt` | استقبال تعليق |
| `UserJoinedStream` | `userName` | مستخدم انضم للبث |
| `UserLeftStream` | `userName` | مستخدم غادر البث |

---

## 📊 ملخص إحصائي

| القسم | عدد الـ Endpoints |
|-------|------------------|
| المصادقة (Auth) | 6 |
| تسجيل الطلاب | 6 |
| المدرسة | 1 |
| السنوات الدراسية | 2 |
| الفروع | 1 |
| المراحل | 1 |
| الشعب | 1 |
| المواد | 1 |
| الفصول | 1 |
| الحضور | 4 |
| درجات الطلاب | 4 |
| الواجبات | 4 |
| الاختبارات | 16 |
| جداول الامتحانات | 4 |
| أنواع الامتحانات | 1 |
| الجدول الأسبوعي | 4 |
| تعيينات المدرسين | 4 |
| الإعلانات | 5 |
| الإشعارات | 6 |
| الأحداث | 1 |
| الإجازات | 4 |
| السلوك | 2 |
| السجلات الصحية | 2 |
| الرواتب | 1 |
| أرباح المدرسين | 5 |
| المحادثات | 7 |
| الكاروسيل | 1 |
| اشتراكات الطلاب | 2 |
| أكواد الخصم | 3 |
| خطط الاشتراك | 2 |
| الكورسات | 6 |
| فيديوهات الكورسات | 24 |
| البث المباشر | 9 |
| المكتبة | 2 |
| الأقساط | 3 |
| HR الموظفين | 8 |
| HR الأقسام | 1 |
| HR المسميات | 1 |
| HR الدرجات | 2 |
| HR الرواتب | 11 |
| HR الحضور | 3 |
| HR العقود | 2 |
| HR الإجازات | 7 |
| HR العمل الإضافي | 1 |
| HR الأداء | 4 |
| HR الترقيات | 2 |
| HR الورديات | 1 |
| HR التدريب | 3 |
| HR التأديبية | 2 |
| **المجموع** | **~181 endpoint** |

---

## 🔑 ملاحظات مهمة للتطبيق المتجاوب

1. **المصادقة:** جميع الطلبات (ماعدا Login و Registration) تتطلب JWT Token في الهيدر.
2. **schoolId:** معظم الـ Routes تتطلب `schoolId` كجزء من URL path.
3. **الفلترة حسب المستخدم:** كثير من الـ Endpoints تُفلتر تلقائياً حسب نوع المستخدم من التوكن.
4. **أنواع المستخدمين:** `Teacher` | `Student` | `Parent` | `Staff`
5. **SignalR:** استخدم للمحادثات والإشعارات والبث المباشر في الوقت الحقيقي.
6. **Rate Limiting:** انتبه لـ 10 طلبات/دقيقة على Auth و 100 طلب/دقيقة على API.
7. **رفع الملفات:** Chat: max 20MB — Videos: max 500MB.
8. **قيد الجهاز للطلاب:** الطالب مقيد بجهاز واحد. استخدم `check-device` و `logout-device`.
9. **الفيديوهات المجدولة:** لا تُعرض URLs حتى موعد النشر.
10. **Push Notifications:** يستخدم OneSignal لإرسال إشعارات Push.
