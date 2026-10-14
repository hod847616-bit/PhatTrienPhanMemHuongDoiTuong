using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== HỆ THỐNG QUẢN LÝ LỚP HỌC ===");

        // Mock Data
        var instructor = Instructor.Create("GV Nguyen Van A");
        var student1 = Student.Create("SV Le Van B");
        var student2 = Student.Create("SV Tran Thi C");

        try
        {
            // US-01: Admin tạo khóa học (Test lỗi MaxCapacity)
            // var badCourse = Course.Create("CS101", "C# Basic", 4, instructor.Id); // Sẽ văng lỗi
            
            var course = Course.Create("CS101", "C# Advanced", 5, instructor.Id);
            Console.WriteLine($"[US-01] Đã tạo khóa học {course.Code} thành công. Sức chứa: {course.MaxCapacity}");

            // US-02: Sinh viên đăng ký tham gia
            course.EnrollStudent(student1.Id);
            course.EnrollStudent(student2.Id);
            Console.WriteLine($"[US-02] {student1.Name} và {student2.Name} đã đăng ký thành công.");
            // course.EnrollStudent(student1.Id); // Thử đăng ký lại sẽ văng lỗi Duplicate

            // US-03: Giảng viên tạo buổi học
            var session1 = course.CreateSession(instructor.Id, DateTime.Now, "OOP in C#");
            var session2 = course.CreateSession(instructor.Id, DateTime.Now.AddDays(2), "LINQ & Lambdas");
            Console.WriteLine($"[US-03] Giảng viên đã tạo 2 buổi học.");

            // US-04: Giảng viên điểm danh
            var attendances = new List<Attendance>
            {
                Attendance.Create(session1.Id, student1.Id, AttendanceStatus.Present, course),
                Attendance.Create(session1.Id, student2.Id, AttendanceStatus.Absent, course),
                
                Attendance.Create(session2.Id, student1.Id, AttendanceStatus.Late, course),
                Attendance.Create(session2.Id, student2.Id, AttendanceStatus.Present, course)
            };
            Console.WriteLine($"[US-04] Đã hoàn tất điểm danh cho các buổi học.");

            // US-05: Sinh viên xem báo cáo tỷ lệ chuyên cần (Mô phỏng cho Student2)
            Console.WriteLine("\n[US-05] === BÁO CÁO CHUYÊN CẦN CỦA " + student2.Name.ToUpper() + " ===");
            var student2Records = attendances.Where(a => a.StudentId == student2.Id).ToList();
            int totalSessions = 2; // Khóa học có 2 buổi
            int presentCount = student2Records.Count(a => a.Status == AttendanceStatus.Present);
            int lateCount = student2Records.Count(a => a.Status == AttendanceStatus.Late);
            int absentCount = student2Records.Count(a => a.Status == AttendanceStatus.Absent);
            
            double absentRate = (double)absentCount / totalSessions * 100;

            Console.WriteLine($"- Tổng số buổi học: {totalSessions}");
            Console.WriteLine($"- Có mặt: {presentCount} | Đi muộn: {lateCount} | Vắng mặt: {absentCount}");
            Console.WriteLine($"- Tỷ lệ vắng mặt: {absentRate}%");

            if (absentRate > 20)
            {
                Console.WriteLine("-> [CẢNH BÁO]: CẤM THI (Tỷ lệ vắng > 20%)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LỖI]: {ex.Message}");
        }
    }
}