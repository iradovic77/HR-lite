import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Table, Button, Tag, Space, Form, Input, Select,
  InputNumber, Switch, App, Pagination, Tooltip, Checkbox,
} from 'antd'
import AppModal from '@/components/AppModal'
import { PlusOutlined, EditOutlined, StopOutlined, CheckOutlined, DeleteOutlined } from '@ant-design/icons'
import type { ColumnsType } from 'antd/es/table'
import {
  municipalityApi, settlementApi,
  type MunicipalityResponse, type SettlementResponse, type CreateSettlementRequest,
} from '@/api/codebook'
import CodebookLayout from '@/layouts/CodebookLayout'

const PAGE_SIZE = 20

interface FormValues {
  code: string
  nameHr: string
  nameEn?: string | null
  municipalityId?: string | null
  ordinal: number
  isActive: boolean
}

export default function SettlementPage() {
  const { t } = useTranslation()
  const { message, modal } = App.useApp()

  const [data, setData]                             = useState<SettlementResponse[]>([])
  const [municipalities, setMunicipalities]         = useState<MunicipalityResponse[]>([])
  const [loading, setLoading]                       = useState(false)
  const [page, setPage]                             = useState(1)
  const [onlyActive, setOnlyActive]                 = useState(true)
  const [municipalityFilter, setMunicipalityFilter] = useState<string | null>(null)
  const [modalOpen, setModalOpen]                   = useState(false)
  const [saving, setSaving]                         = useState(false)
  const [editingItem, setEditingItem]               = useState<SettlementResponse | null>(null)
  const [form] = Form.useForm<FormValues>()

  const fetchMunicipalities = async () => {
    try {
      const res = await municipalityApi.getAll(false)
      setMunicipalities(res.data)
    } catch { /* silent */ }
  }

  const fetchData = async () => {
    setLoading(true)
    try {
      const res = await settlementApi.getAll(true, municipalityFilter)
      setData(res.data)
      setPage(1)
    } catch {
      message.error('Greška pri dohvatu podataka')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchMunicipalities() }, [])
  useEffect(() => { fetchData() }, [municipalityFilter])

  const filteredData  = onlyActive ? data.filter(d => d.isActive) : data
  const paginatedData = filteredData.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  const openAddModal = () => {
    setEditingItem(null)
    form.resetFields()
    form.setFieldsValue({ isActive: true, ordinal: data.length + 1 })
    setModalOpen(true)
  }

  const openEditModal = (item: SettlementResponse) => {
    setEditingItem(item)
    form.setFieldsValue({
      code: item.code,
      nameHr: item.nameHr,
      nameEn: item.nameEn,
      municipalityId: item.municipalityId,
      ordinal: item.ordinal,
      isActive: item.isActive,
    })
    setModalOpen(true)
  }

  const handleSave = async () => {
    const values = await form.validateFields()
    setSaving(true)
    try {
      if (editingItem) {
        await settlementApi.update(editingItem.id, values)
        message.success('Zapis ažuriran')
      } else {
        await settlementApi.create(values as CreateSettlementRequest)
        message.success('Zapis dodan')
      }
      setModalOpen(false)
      fetchData()
    } catch {
      message.error('Greška pri spremanju')
    } finally {
      setSaving(false)
    }
  }

  const handleToggleActive = async (item: SettlementResponse) => {
    try {
      await settlementApi.toggleActive(item.id)
      message.success(item.isActive ? 'Zapis deaktiviran' : 'Zapis aktiviran')
      fetchData()
    } catch {
      message.error('Greška pri promjeni statusa')
    }
  }

  const handleDelete = async (item: SettlementResponse) => {
    try {
      await settlementApi.deleteById(item.id)
      message.success('Zapis obrisan')
      fetchData()
    } catch (err: unknown) {
      const status = (err as { response?: { status?: number } })?.response?.status
      message.error(status === 409 ? t('common.error_in_use') : t('common.error_delete'))
    }
  }

  const confirmToggleActive = (item: SettlementResponse) => {
    modal.confirm({
      title: t(item.isActive
        ? 'codebook.city.confirm.deactivate'
        : 'codebook.city.confirm.activate'),
      onOk: () => handleToggleActive(item),
      okText: t('common.yes'),
      cancelText: t('common.no'),
      centered: true,
    })
  }

  const confirmDelete = (item: SettlementResponse) => {
    modal.confirm({
      title: t('codebook.city.confirm.delete'),
      content: t('codebook.city.confirm.delete_description'),
      onOk: () => handleDelete(item),
      okText: t('codebook.city.confirm.delete_ok'),
      okType: 'danger',
      cancelText: t('common.no'),
      centered: true,
    })
  }

  const columns: ColumnsType<SettlementResponse> = [
    {
      title: t('codebook.city.columns.code'),
      dataIndex: 'code',
      key: 'code',
      width: 80,
      sorter: (a, b) => a.code.localeCompare(b.code),
    },
    {
      title: t('codebook.city.columns.nameHr'),
      dataIndex: 'nameHr',
      key: 'nameHr',
    },
    {
      title: t('codebook.city.columns.nameEn'),
      dataIndex: 'nameEn',
      key: 'nameEn',
      render: (val: string | null) => val ?? '—',
    },
    {
      title: t('codebook.city.columns.municipality'),
      dataIndex: 'municipalityNameHr',
      key: 'municipalityNameHr',
      render: (val: string | null) => val ?? '—',
    },
    {
      title: t('codebook.city.columns.ordinal'),
      dataIndex: 'ordinal',
      key: 'ordinal',
      width: 110,
      sorter: (a, b) => a.ordinal - b.ordinal,
      defaultSortOrder: 'ascend',
    },
    {
      title: t('codebook.city.columns.status'),
      dataIndex: 'isActive',
      key: 'isActive',
      width: 110,
      render: (isActive: boolean) =>
        isActive
          ? <Tag color="success">{t('codebook.city.status.active')}</Tag>
          : <Tag color="default">{t('codebook.city.status.inactive')}</Tag>,
    },
    {
      title: t('codebook.city.columns.actions'),
      key: 'actions',
      width: 120,
      render: (_, record) => (
        <Space size={4}>
          <Tooltip title={t('codebook.city.actions.edit')}>
            <Button type="text" icon={<EditOutlined />} size="small"
              onClick={() => openEditModal(record)} />
          </Tooltip>
          <Tooltip title={t(record.isActive
            ? 'codebook.city.actions.deactivate'
            : 'codebook.city.actions.activate')}
          >
            <Button type="text"
              icon={record.isActive ? <StopOutlined /> : <CheckOutlined />}
              size="small" danger={record.isActive}
              onClick={() => confirmToggleActive(record)} />
          </Tooltip>
          <Tooltip title={t('codebook.city.actions.delete')}>
            <Button type="text" icon={<DeleteOutlined />} size="small" danger
              onClick={() => confirmDelete(record)} />
          </Tooltip>
        </Space>
      ),
    },
  ]

  const municipalityOptions = municipalities.map(m => ({ value: m.id, label: m.nameHr }))

  return (
    <CodebookLayout
      title={t('codebook.city.title')}
      extra={
        <Space>
          <Select
            allowClear
            placeholder={t('codebook.city.filter_municipality')}
            value={municipalityFilter}
            onChange={(v) => { setMunicipalityFilter(v ?? null); setPage(1) }}
            options={municipalityOptions}
            style={{ minWidth: 160 }}
            size="small"
          />
          <Checkbox
            checked={onlyActive}
            onChange={e => { setOnlyActive(e.target.checked); setPage(1) }}
          >
            {t('common.only_active')}
          </Checkbox>
          <Button type="primary" icon={<PlusOutlined />} onClick={openAddModal}>
            {t('codebook.city.addNew')}
          </Button>
        </Space>
      }
      pagination={
        <Pagination
          current={page}
          total={filteredData.length}
          pageSize={PAGE_SIZE}
          onChange={setPage}
          showTotal={(total) => `Ukupno: ${total}`}
          size="small"
        />
      }
    >
      <Table
        columns={columns}
        dataSource={paginatedData}
        rowKey="id"
        size="small"
        loading={loading}
        pagination={false}
        onRow={(record) => ({
          style: record.isActive ? {} : { opacity: 0.45 },
        })}
      />

      <AppModal
        title={editingItem
          ? t('codebook.city.modal.editTitle')
          : t('codebook.city.modal.addTitle')}
        open={modalOpen}
        onOk={handleSave}
        onCancel={() => setModalOpen(false)}
        okText={t('common.save')}
        cancelText={t('common.cancel')}
        confirmLoading={saving}
        destroyOnClose
      >
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Form.Item
            name="code"
            label={t('codebook.city.modal.code')}
            tooltip={t('codebook.city.modal.code_hint')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={20} style={{ textTransform: 'uppercase' }} />
          </Form.Item>
          <Form.Item
            name="nameHr"
            label={t('codebook.city.modal.nameHr')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="nameEn" label={t('codebook.city.modal.nameEn')}>
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="municipalityId" label={t('codebook.city.modal.municipality')}>
            <Select allowClear options={municipalityOptions} />
          </Form.Item>
          <Form.Item name="ordinal" label={t('codebook.city.modal.ordinal')}>
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="isActive" label={t('codebook.city.modal.isActive')} valuePropName="checked">
            <Switch />
          </Form.Item>
        </Form>
      </AppModal>
    </CodebookLayout>
  )
}
