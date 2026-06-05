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
  countyApi, municipalityApi,
  type CountyResponse, type MunicipalityResponse, type CreateMunicipalityRequest,
} from '@/api/codebook'
import CodebookLayout from '@/layouts/CodebookLayout'

const PAGE_SIZE = 20

interface FormValues {
  code: string
  nameHr: string
  nameEn?: string | null
  countyId?: string | null
  ordinal: number
  isActive: boolean
}

export default function MunicipalityPage() {
  const { t } = useTranslation()
  const { message, modal } = App.useApp()

  const [data, setData]                 = useState<MunicipalityResponse[]>([])
  const [counties, setCounties]         = useState<CountyResponse[]>([])
  const [loading, setLoading]           = useState(false)
  const [page, setPage]                 = useState(1)
  const [onlyActive, setOnlyActive]     = useState(true)
  const [countyFilter, setCountyFilter] = useState<string | null>(null)
  const [modalOpen, setModalOpen]       = useState(false)
  const [saving, setSaving]             = useState(false)
  const [editingItem, setEditingItem]   = useState<MunicipalityResponse | null>(null)
  const [form] = Form.useForm<FormValues>()

  const fetchCounties = async () => {
    try {
      const res = await countyApi.getAll(false)
      setCounties(res.data)
    } catch { /* silent */ }
  }

  const fetchData = async () => {
    setLoading(true)
    try {
      const res = await municipalityApi.getAll(true, countyFilter)
      setData(res.data)
      setPage(1)
    } catch {
      message.error('Greška pri dohvatu podataka')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchCounties() }, [])
  useEffect(() => { fetchData() }, [countyFilter])

  const filteredData  = onlyActive ? data.filter(d => d.isActive) : data
  const paginatedData = filteredData.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  const openAddModal = () => {
    setEditingItem(null)
    form.resetFields()
    form.setFieldsValue({ isActive: true, ordinal: data.length + 1 })
    setModalOpen(true)
  }

  const openEditModal = (item: MunicipalityResponse) => {
    setEditingItem(item)
    form.setFieldsValue({
      code: item.code,
      nameHr: item.nameHr,
      nameEn: item.nameEn,
      countyId: item.countyId,
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
        await municipalityApi.update(editingItem.id, values)
        message.success('Zapis ažuriran')
      } else {
        await municipalityApi.create(values as CreateMunicipalityRequest)
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

  const handleToggleActive = async (item: MunicipalityResponse) => {
    try {
      await municipalityApi.toggleActive(item.id)
      message.success(item.isActive ? 'Zapis deaktiviran' : 'Zapis aktiviran')
      fetchData()
    } catch {
      message.error('Greška pri promjeni statusa')
    }
  }

  const handleDelete = async (item: MunicipalityResponse) => {
    try {
      await municipalityApi.deleteById(item.id)
      message.success('Zapis obrisan')
      fetchData()
    } catch (err: unknown) {
      const status = (err as { response?: { status?: number } })?.response?.status
      message.error(status === 409 ? t('common.error_in_use') : t('common.error_delete'))
    }
  }

  const confirmToggleActive = (item: MunicipalityResponse) => {
    modal.confirm({
      title: t(item.isActive
        ? 'codebook.municipality.confirm.deactivate'
        : 'codebook.municipality.confirm.activate'),
      onOk: () => handleToggleActive(item),
      okText: t('common.yes'),
      cancelText: t('common.no'),
      centered: true,
    })
  }

  const confirmDelete = (item: MunicipalityResponse) => {
    modal.confirm({
      title: t('codebook.municipality.confirm.delete'),
      content: t('codebook.municipality.confirm.delete_description'),
      onOk: () => handleDelete(item),
      okText: t('codebook.municipality.confirm.delete_ok'),
      okType: 'danger',
      cancelText: t('common.no'),
      centered: true,
    })
  }

  const columns: ColumnsType<MunicipalityResponse> = [
    {
      title: t('codebook.municipality.columns.code'),
      dataIndex: 'code',
      key: 'code',
      width: 80,
      sorter: (a, b) => a.code.localeCompare(b.code),
    },
    {
      title: t('codebook.municipality.columns.nameHr'),
      dataIndex: 'nameHr',
      key: 'nameHr',
    },
    {
      title: t('codebook.municipality.columns.nameEn'),
      dataIndex: 'nameEn',
      key: 'nameEn',
      render: (val: string | null) => val ?? '—',
    },
    {
      title: t('codebook.municipality.columns.county'),
      dataIndex: 'countyNameHr',
      key: 'countyNameHr',
      render: (val: string | null) => val ?? '—',
    },
    {
      title: t('codebook.municipality.columns.ordinal'),
      dataIndex: 'ordinal',
      key: 'ordinal',
      width: 110,
      sorter: (a, b) => a.ordinal - b.ordinal,
      defaultSortOrder: 'ascend',
    },
    {
      title: t('codebook.municipality.columns.status'),
      dataIndex: 'isActive',
      key: 'isActive',
      width: 110,
      render: (isActive: boolean) =>
        isActive
          ? <Tag color="success">{t('codebook.municipality.status.active')}</Tag>
          : <Tag color="default">{t('codebook.municipality.status.inactive')}</Tag>,
    },
    {
      title: t('codebook.municipality.columns.actions'),
      key: 'actions',
      width: 120,
      render: (_, record) => (
        <Space size={4}>
          <Tooltip title={t('codebook.municipality.actions.edit')}>
            <Button type="text" icon={<EditOutlined />} size="small"
              onClick={() => openEditModal(record)} />
          </Tooltip>
          <Tooltip title={t(record.isActive
            ? 'codebook.municipality.actions.deactivate'
            : 'codebook.municipality.actions.activate')}
          >
            <Button type="text"
              icon={record.isActive ? <StopOutlined /> : <CheckOutlined />}
              size="small" danger={record.isActive}
              onClick={() => confirmToggleActive(record)} />
          </Tooltip>
          <Tooltip title={t('codebook.municipality.actions.delete')}>
            <Button type="text" icon={<DeleteOutlined />} size="small" danger
              onClick={() => confirmDelete(record)} />
          </Tooltip>
        </Space>
      ),
    },
  ]

  const countyOptions = counties.map(c => ({ value: c.id, label: c.nameHr }))

  return (
    <CodebookLayout
      title={t('codebook.municipality.title')}
      extra={
        <Space>
          <Select
            allowClear
            placeholder={t('codebook.municipality.filter_county')}
            value={countyFilter}
            onChange={(v) => { setCountyFilter(v ?? null); setPage(1) }}
            options={countyOptions}
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
            {t('codebook.municipality.addNew')}
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
          ? t('codebook.municipality.modal.editTitle')
          : t('codebook.municipality.modal.addTitle')}
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
            label={t('codebook.municipality.modal.code')}
            tooltip={t('codebook.municipality.modal.code_hint')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={20} style={{ textTransform: 'uppercase' }} />
          </Form.Item>
          <Form.Item
            name="nameHr"
            label={t('codebook.municipality.modal.nameHr')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="nameEn" label={t('codebook.municipality.modal.nameEn')}>
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="countyId" label={t('codebook.municipality.modal.county')}>
            <Select allowClear options={countyOptions} />
          </Form.Item>
          <Form.Item name="ordinal" label={t('codebook.municipality.modal.ordinal')}>
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="isActive" label={t('codebook.municipality.modal.isActive')} valuePropName="checked">
            <Switch />
          </Form.Item>
        </Form>
      </AppModal>
    </CodebookLayout>
  )
}
