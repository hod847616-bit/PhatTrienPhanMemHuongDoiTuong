using System;
using System.Collections.Generic;
using System.Linq;

public enum AttendanceStatus { Present, Absent, Late }

public class Instructor
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private Instructor() { }
    public static Instructor Create(string name) => new Instructor { Id = Guid.NewGuid(), Name = name };
}

public class Student
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private Student() { }
    public static Student Create(string name) => new Student { Id = Guid.NewGuid(), Name = name };
}

public class Course
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public int MaxCapacity { get; private set; }
    public Guid InstructorId { get; private set; }
    
    private readonly List<Guid> _enrolledStudentIds = new List<Guid>();
    public IReadOnlyList<Guid> EnrolledStudentIds => _enrolledStudentIds.AsReadOnly();

    private Course() { }

    // US-01: Admin tạo khóa học
    public static Course Create(string code, string name, int maxCapacity, Guid instructorId)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Mã và tên khóa học không được để trống.");
        if (maxCapacity < 5 || maxCapacity > 100)
            throw new ArgumentException("MaxCapacity phải nằm trong khoảng [5..100].");
        if (instructorId == Guid.Empty)
            throw new ArgumentException("InstructorId không hợp lệ.");

        return new Course
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name,
            MaxCapacity = maxCapacity,
            InstructorId = instructorId
        };
    }

    // US-02: Sinh viên đăng ký
    public void EnrollStudent(Guid studentId)
    {
        if (_enrolledStudentIds.Contains(studentId))
            throw new InvalidOperationException("Sinh viên đã đăng ký khóa học này.");
        if (_enrolledStudentIds.Count >= MaxCapacity)
            throw new InvalidOperationException("Khóa học đã đạt số lượng tối đa (Hết chỗ).");

        _enrolledStudentIds.Add(studentId);
    }

    // US-03: Giảng viên tạo buổi học
    public Session CreateSession(Guid instructorId, DateTime date, string topic)
    {
        if (this.InstructorId != instructorId)
            throw new UnauthorizedAccessException("Chỉ giảng viên phụ trách mới được tạo buổi học.");
        
        return Session.Create(this.Id, date, topic);
    }
}

public class Session
{
    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public DateTime Date { get; private set; }
    public string Topic { get; private set; }

    private Session() { }

    public static Session Create(Guid courseId, DateTime date, string topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Chủ đề buổi học không được để trống.");

        return new Session
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            Date = date,
            Topic = topic
        };
    }
}

public class Attendance
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public Guid StudentId { get; private set; }
    public AttendanceStatus Status { get; private set; }

    private Attendance() { }

    // US-04: Giảng viên điểm danh
    public static Attendance Create(Guid sessionId, Guid studentId, AttendanceStatus status, Course course)
    {
        if (!course.EnrolledStudentIds.Contains(studentId))
            throw new InvalidOperationException("Sinh viên không có trong danh sách lớp hợp lệ.");

        return new Attendance
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            StudentId = studentId,
            Status = status
        };
    }
}