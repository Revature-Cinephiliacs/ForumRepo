using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Dbcontext for Forum Api database
    /// </summary>
    public partial class Cinephiliacs_ForumContext : DbContext
    {
        public Cinephiliacs_ForumContext()
        {
        }

        public Cinephiliacs_ForumContext(DbContextOptions<Cinephiliacs_ForumContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Discussion> Discussions { get; set; }
        public virtual DbSet<DiscussionTopic> DiscussionTopics { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comments");

                entity.Property(e => e.CommentId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("commentID");

                entity.Property(e => e.CommentText)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("comment_text");

                entity.Property(e => e.CreationTime)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_time");

                entity.Property(e => e.DiscussionId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("discussionID");

                entity.Property(e => e.IsSpoiler).HasColumnName("is_spoiler");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.HasOne(d => d.Discussion)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.DiscussionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__comments__discus__6EF57B66");
            });

            modelBuilder.Entity<Discussion>(entity =>
            {
                entity.ToTable("discussions");

                entity.Property(e => e.DiscussionId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("discussionID");

                entity.Property(e => e.CreationTime)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_time");

                entity.Property(e => e.MovieId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("movieID");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("subject");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<DiscussionTopic>(entity =>
            {
                entity.HasKey(e => new { e.DiscussionId, e.TopicName })
                    .HasName("discussionID_topic_pk");

                entity.ToTable("discussion_topics");

                entity.Property(e => e.DiscussionId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("discussionID");

                entity.Property(e => e.TopicName)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("topic_name");

                entity.HasOne(d => d.Discussion)
                    .WithMany(p => p.DiscussionTopics)
                    .HasForeignKey(d => d.DiscussionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__discussio__discu__6B24EA82");

                entity.HasOne(d => d.TopicNameNavigation)
                    .WithMany(p => p.DiscussionTopics)
                    .HasForeignKey(d => d.TopicName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__discussio__topic__6C190EBB");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.HasKey(e => e.Setting1)
                    .HasName("PK__settings__25A3BB9AE55A1B57");

                entity.ToTable("settings");

                entity.Property(e => e.Setting1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("setting");

                entity.Property(e => e.IntValue).HasColumnName("intValue");

                entity.Property(e => e.StringValue)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("stringValue");
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.HasKey(e => e.TopicName)
                    .HasName("PK__topics__54BAE5ED445F65A9");

                entity.ToTable("topics");

                entity.Property(e => e.TopicName)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("topic_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
