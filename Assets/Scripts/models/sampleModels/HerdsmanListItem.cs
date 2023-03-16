using System.Collections;
using System.Collections.Generic;
public class HerdsmanListItem
{
    /// <summary>
    /// ��ǰ�������ܱ��
    /// </summary>
    public string rackCode { get; set; }

    /// <summary>
    /// ��ǰ��������
    /// </summary>
    public string bagCode;
    /// <summary>
    /// ������ӵ�еı�ǩ�б�
    /// </summary>
    public List<Taginfo> tagCodeList { get; set; }
    /// <summary>
    /// ����ID
    /// </summary>
    public string herdsmanId { get; set; }
    /// <summary>
    /// ������ӵ�е���Ʒ����
    /// </summary>
    public int sampleQuantity { get; set; }
    /// <summary>
    /// ������
    /// </summary>
    public string herdsmanCode { get; set; }
    /// <summary>
    /// ����
    /// </summary>
    public string herdsmanName { get; set; }
    /// <summary>
    /// ����ȱ�ٵ���Ʒ����
    /// </summary>
    public int lackSampleNum{get;set;}
    /// <summary>
    /// ʵ�����������
    /// </summary>
    public int actualSampleNum { get; set; }
    /// <summary>
    /// ��Ӧ��Ʒ����
    /// </summary>
    public int detectSampleNum { get; set; }
    /// <summary>
    /// ȱ�ٶ�������
    /// </summary>
   public int earCodeLackSampleNum { get; set; }
   /// <summary>
   /// ������Ʒ����
   /// </summary>
    public int herdsmanSampleSum { get; set; }
    /// <summary>
    /// ȱ�ٶ����б�
    /// </summary>
    public List<string> noEarCodeList { get; set; }
}