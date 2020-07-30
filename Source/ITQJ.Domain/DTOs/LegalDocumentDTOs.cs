﻿namespace ITQJ.Domain.DTOs
{
    public class LegalDocumentCreateDTO
    {
        public string Number { get; set; }

        public byte[] Image { get; set; }

        public int DocumentTypeId { get; set; }
    }

    public class LegalDocumentUpdateDTO : LegalDocumentCreateDTO
    {

    }

    public class LegalDocumentResponseDTO : LegalDocumentCreateDTO
    {
        public int Id { get; set; }
        public DocumentTypeDTO DocumentType { get; set; }
    }
}